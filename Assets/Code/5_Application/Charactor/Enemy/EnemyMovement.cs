using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Core")]
    public float speed = 3f;
    public float turnSharpness = 8f;

    [Header("Obstacle Avoidance")]
    public float avoidDistance = 0.9f;
    public float avoidForce = 0.4f;
    public LayerMask obstacleMask;

    [Header("Separation")]
    public float separationRadius = 0.7f;
    public float separationForce = 0.6f;
    public float minSpacing = 0.5f;
    public float hardPushStrength = 0.8f;

    [Header("Path Following")]
    public float waypointReachThreshold = 0.22f;
    public float lookaheadDistance = 0.6f;

    [Header("Debug")]
    public bool debugDraw = false;

    private Rigidbody2D _rb;
    public BoxCollider2D _box;
    private Vector2 _halfExtents;
    private Vector2 _smoothedDir;
    private bool _isDash = false;

    private RaycastHit2D[] _rayBuffer = new RaycastHit2D[4];
    private readonly Queue<Vector3> _path = new Queue<Vector3>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_box == null) _box = GetComponent<BoxCollider2D>();
        _halfExtents = _box != null ? _box.size * 0.5f : Vector2.one * 0.5f;
    }

    // DASH
    public void ApplyImpulse(Vector2 impulse, float duration)
    {
        _isDash = true;
        _rb.velocity = impulse;
        StopCoroutine(nameof(DashCancelRoutine));
        StartCoroutine(DashCancelRoutine(duration));
    }

    private IEnumerator DashCancelRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isDash = false;
        _rb.velocity = Vector2.zero;
    }

    // PATH API
    public void SetPath(List<Vector3> worldPath)
    {
        _path.Clear();
        if (worldPath == null || worldPath.Count == 0) return;
        foreach (var p in worldPath) _path.Enqueue(p);
    }

    public void ClearPath() => _path.Clear();
    public bool HasPath => _path.Count > 0;

    public Vector3 PeekWaypoint() => _path.Count == 0 ? transform.position : _path.Peek();

    public List<Vector3> GetPathCopy() => new List<Vector3>(_path);

    public float CurrentSpeed => _rb != null ? _rb.velocity.magnitude : 0f;

    // FOLLOW PATH
    public void FollowPath()
    {
        if (_isDash) return;
        if (_path.Count == 0) return;

        Vector3 look = ComputeLookaheadPoint(lookaheadDistance);
        Vector2 dir = (look - transform.position);
        float dist = dir.magnitude;

        Vector3 frontWp = _path.Peek();
        float distToFront = Vector3.Distance(transform.position, frontWp);
        if (distToFront > 2.2f) return; // drift guarded; let ChaseState repath

        if (distToFront <= waypointReachThreshold)
        {
            _path.Dequeue();
            if (_path.Count == 0)
            {
                _rb.velocity = Vector2.zero;
                return;
            }
            look = ComputeLookaheadPoint(lookaheadDistance);
            dir = (look - transform.position);
        }

        MoveTowards(dir);
    }

    private Vector3 ComputeLookaheadPoint(float lookahead)
    {
        if (_path.Count == 0) return transform.position;
        if (_path.Count == 1) return _path.Peek();

        List<Vector3> list = GetPathCopy();
        Vector3 prev = transform.position;
        float acc = 0f;
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 wp = list[i];
            float seg = Vector3.Distance(prev, wp);
            if (acc + seg >= lookahead)
            {
                float rem = lookahead - acc;
                return prev + (wp - prev).normalized * rem;
            }
            acc += seg;
            prev = wp;
        }
        return list[list.Count - 1];
    }

    // Steering main
    public void MoveTowards(Vector2 desiredDirection)
    {
        if (_isDash) return;
        if (desiredDirection.sqrMagnitude < 0.0001f) { _rb.velocity = Vector2.zero; return; }

        Vector2 forward = desiredDirection.normalized;
        Vector2 avoidVec = HasPath ? Vector2.zero : BoxAvoidance(forward);
        Vector2 separationVec = ComputeSeparationFromGrid();

        Vector2 final = forward + avoidVec + separationVec * separationForce;
        if (final.sqrMagnitude > 1f) final.Normalize();

        _smoothedDir = Vector2.Lerp(_smoothedDir, final, Time.deltaTime * turnSharpness);
        _rb.velocity = _smoothedDir * speed;
    }

    public void Stop() => _rb.velocity = Vector2.zero;

    // Box avoidance
    private Vector2 BoxAvoidance(Vector2 forward)
    {
        Vector2 pos = transform.position;
        Vector2 perp = Vector2.Perpendicular(forward).normalized;
        float dist = avoidDistance;

        Vector2 lt = pos + new Vector2(-_halfExtents.x, +_halfExtents.y);
        Vector2 lb = pos + new Vector2(-_halfExtents.x, -_halfExtents.y);
        Vector2 rt = pos + new Vector2(+_halfExtents.x, +_halfExtents.y);
        Vector2 rb = pos + new Vector2(+_halfExtents.x, -_halfExtents.y);

        bool hitCenter = Physics2D.RaycastNonAlloc(pos, forward, _rayBuffer, dist, obstacleMask) > 0;
        bool hitLeft = Physics2D.RaycastNonAlloc(lt, forward, _rayBuffer, dist, obstacleMask) > 0 ||
                       Physics2D.RaycastNonAlloc(lb, forward, _rayBuffer, dist, obstacleMask) > 0;
        bool hitRight = Physics2D.RaycastNonAlloc(rt, forward, _rayBuffer, dist, obstacleMask) > 0 ||
                        Physics2D.RaycastNonAlloc(rb, forward, _rayBuffer, dist, obstacleMask) > 0;

        if (!hitCenter && !hitLeft && !hitRight) return Vector2.zero;

        Vector2 avoid = Vector2.zero;
        if (hitCenter)
        {
            if (!hitLeft) avoid += -perp;
            else if (!hitRight) avoid += perp;
            else avoid += (Random.value > 0.5f ? perp : -perp);
        }
        else if (hitLeft && !hitRight) avoid += perp;
        else if (!hitLeft && hitRight) avoid += -perp;

        return avoid * avoidForce;
    }

    // Separation using EnemyManager's spatial query
    private Vector2 ComputeSeparationFromGrid()
    {
        Vector2 pos = transform.position;
        var results = EnemyManager.Instance.QueryRadius(pos, separationRadius);
        if (results == null || results.Count == 0) return Vector2.zero;

        Vector2 force = Vector2.zero;
        for (int i = 0; i < results.Count; i++)
        {
            var e = results[i];
            if (e == null) continue;
            if (e.transform == transform) continue;

            Vector2 diff = pos - (Vector2)e.transform.position;
            float dist = diff.magnitude;
            if (dist < 0.001f) continue;

            float push = 1f - (dist / separationRadius);
            force += diff.normalized * push;

            if (dist < minSpacing)
            {
                float penetration = minSpacing - dist;
                force += diff.normalized * penetration * hardPushStrength;
            }
        }
        return force;
    }

    // Debug gizmos
    void OnDrawGizmos()
    {
        if (!debugDraw || _box == null) return;
        Vector2 pos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + _smoothedDir * 0.8f);

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireCube(pos + _box.offset, _box.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, separationRadius);

        if (_path.Count > 0)
        {
            Gizmos.color = Color.magenta;
            Vector3 prev = transform.position;
            foreach (var wp in _path)
            {
                Gizmos.DrawLine(prev, wp);
                Gizmos.DrawSphere(wp, 0.06f);
                prev = wp;
            }
        }
    }
}
