using Codice.Client.BaseCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float turnSharpness = 8f;
    public float velocitySmooth = 12f;

    [Header("Avoidance")]
    public float avoidDistance = 0.9f;
    public float avoidForce = 0.42f;
    public LayerMask obstacleMask;

    [Header("Crowd / Separation")]
    public float separationRadius = 0.7f;
    public float separationForce = 0.6f;
    public float minSpacing = 0.55f;

    [Header("Lookahead Navigation")]
    public float lookaheadDistance = 0.55f;
    public float waypointReachThreshold = 0.24f;

    [Header("Debug")]
    public bool debugDraw = false;

    // internal
    private Rigidbody2D _rb;
    private BoxCollider2D _box;
    private Vector2 _halfExtents;
    private EnemyCrowdAgent _crowd;

    private Vector2 _smoothedDir;
    private Vector2 _velBlend;
    private bool _isDash;

    // path
    private readonly Queue<Vector3> _path = new();
    public bool HasPath => _path.Count > 0;
    public Vector3 PeekWaypoint() => _path.Count > 0 ? _path.Peek() : transform.position;

    // --- Stuck Detection ---
    public bool IsStuck => _isStuck;
    public bool StuckJustTriggered => _stuckJustTriggered;
    public float StuckTime => _stuckTimer;

    private bool _isStuck;
    private bool _stuckJustTriggered;
    private float _stuckTimer;
    private Vector2 _lastPos;

    private const float EXPECTED_SPEED_FACTOR = 0.1f;
    private const float STUCK_TIME_THRESHOLD = 0.28f;

    private RaycastHit2D[] _ray = new RaycastHit2D[4];

    // ==========================================================
    // UNITY
    // ==========================================================
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _crowd = GetComponent<EnemyCrowdAgent>();

        _halfExtents = _box.size * 0.5f;
        _lastPos = transform.position;
    }

    // ==========================================================
    // PATH API
    // ==========================================================
    public void SetPath(List<Vector3> worldPath)
    {
        _path.Clear();
        if (worldPath == null || worldPath.Count == 0)
            return;

        foreach (var p in worldPath)
            _path.Enqueue(p);

        ResetStuck();
    }

    public void ClearPath()
    {
        _path.Clear();
    }

    // ==========================================================
    // FOLLOW PATH
    // ==========================================================
    public void FollowPath()
    {
        if (_isDash || _path.Count == 0)
            return;

        Vector3 next = _path.Peek();
        float dist = Vector3.Distance(transform.position, next);

        // drift guard — path stale, let ChaseState repath
        if (dist > 2.2f) return;

        // reached waypoint
        if (dist <= waypointReachThreshold)
        {
            _path.Dequeue();
            if (_path.Count == 0)
            {
                Stop();
                return;
            }

            next = _path.Peek();
        }

        Vector3 lookTarget = ComputeLookaheadPoint(lookaheadDistance);
        Vector2 desired = lookTarget - transform.position;

        UpdateStuckDetection();

        if (StuckJustTriggered)
        {
            Stop();
            return;
        }

        MoveTowards(desired);
    }

    private Vector3 ComputeLookaheadPoint(float dist)
    {
        if (_path.Count == 0) return transform.position;

        Vector3 front = _path.Peek();
        Vector3 dir = (front - transform.position).normalized;
        return front + dir * dist;
    }

    // ==========================================================
    // MOVEMENT + AVOIDANCE
    // ==========================================================
    public void MoveTowards(Vector2 desiredDirection)
    {
        if (_isDash) return;
        if (desiredDirection.sqrMagnitude < 1e-4f)
        {
            Stop();
            return;
        }

        Vector2 forward = desiredDirection.normalized;

        // Layered AAA steering
        Vector2 avoid = BoxAvoidance(forward);
        Vector2 sep = ComputeSeparation();
        Vector2 final = forward + avoid + sep * separationForce;

        // Corner slide (anti-micro-stuck)
        final += ComputeCornerSlide(forward);

        if (final.sqrMagnitude > 1f)
            final.Normalize();

        _smoothedDir = Vector2.Lerp(_smoothedDir, final, Time.deltaTime * turnSharpness);

        Vector2 targetVel = _smoothedDir * speed;
        _velBlend = Vector2.Lerp(_velBlend, targetVel, Time.deltaTime * velocitySmooth);

        if (_crowd != null)
            _crowd.DesiredVelocity = _velBlend;
        else
            _rb.velocity = _velBlend;
    }

    // ==========================================================
    // AVOIDANCE RAYS (AAA)
    // ==========================================================
    private Vector2 BoxAvoidance(Vector2 forward)
    {
        Vector2 origin = transform.position;
        float dist = avoidDistance;
        Vector2 perp = new Vector2(-forward.y, forward.x);

        Vector2[] dirs =
        {
            forward,
            (forward + perp*0.55f).normalized,
            (forward - perp*0.55f).normalized
        };

        Vector2 total = Vector2.zero;

        for (int i = 0; i < dirs.Length; i++)
        {
            int hits = Physics2D.RaycastNonAlloc(origin, dirs[i], _ray, dist, obstacleMask);
            if (hits > 0)
            {
                // push away
                total += -dirs[i] * avoidForce;

                // detect corner → add normal-based slide
                Vector2 n = _ray[0].normal;
                if (Vector2.Dot(forward, n) < 0.2f)
                    total += Vector2.Perpendicular(n).normalized * 0.45f;

                if (debugDraw)
                    Debug.DrawLine(origin, origin + dirs[i] * dist, Color.red);
            }
            else if (debugDraw)
                Debug.DrawLine(origin, origin + dirs[i] * dist, Color.green);
        }

        return total;
    }

    // ==========================================================
    // CORNER SLIDE (extra anti-snag)
    // ==========================================================
    private Vector2 ComputeCornerSlide(Vector2 forward)
    {
        Vector2 origin = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            forward,
            avoidDistance * 0.8f,
            obstacleMask
        );

        if (hit.collider != null)
        {
            Vector2 n = hit.normal;

            Vector2 slide = Vector2.Perpendicular(n).normalized;

            if (Vector2.Dot(slide, forward) < 0)
                slide = -slide;

            return slide * 0.4f;
        }

        return Vector2.zero;
    }
 
    // ==========================================================
    // CROWD SEPARATION
    // ==========================================================
    private Vector2 ComputeSeparation()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 push = Vector2.zero;
        int count = 0;

        foreach (var c in cols)
        {
            if (c.attachedRigidbody == _rb) continue;

            if (c.CompareTag("Enemy"))
            {
                Vector2 dir = (Vector2)transform.position - (Vector2)c.transform.position;
                float d = dir.magnitude;
                if (d < minSpacing)
                    push += dir.normalized * (minSpacing - d);

                count++;
            }
        }

        if (count > 0)
            push /= count;

        return push;
    }

    // ==========================================================
    // STUCK DETECTION
    // ==========================================================
    private void UpdateStuckDetection()
    {
        Vector2 pos = transform.position;
        float moved = Vector2.Distance(pos, _lastPos);

        float expected = speed * Time.fixedDeltaTime * EXPECTED_SPEED_FACTOR;
        bool tooSlow = moved < expected;

        if (tooSlow)
        {
            _stuckTimer += Time.fixedDeltaTime;
            if (!_isStuck && _stuckTimer >= STUCK_TIME_THRESHOLD)
            {
                _isStuck = true;
                _stuckJustTriggered = true;
            }
        }
        else
        {
            _stuckTimer = 0f;
            _isStuck = false;
            _stuckJustTriggered = false;
        }

        _lastPos = pos;
    }

    public void ResetStuck()
    {
        _isStuck = false;
        _stuckJustTriggered = false;
        _stuckTimer = 0f;
        _lastPos = transform.position;
    }

    // ==========================================================
    // STOP & DASH
    // ==========================================================
    public void Stop()
    {
        _velBlend = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }

    public void ApplyImpulse(Vector2 impulse, float duration)
    {
        if (_isDash) return;

        _isDash = true;
        _rb.velocity = impulse;

        StartCoroutine(DashEnd(duration));
    }

    private IEnumerator DashEnd(float t)
    {
        yield return new WaitForSeconds(t);
        _isDash = false;
        _rb.velocity = Vector2.zero;
    }

    // ==========================================================
    // DEBUG
    // ==========================================================
    private void OnDrawGizmos()
    {
        if (!debugDraw) return;

        if (_path != null && _path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Vector3 prev = transform.position;
            foreach (var p in _path)
            {
                Gizmos.DrawSphere(p, 0.07f);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
        }

        Gizmos.color = new Color(1f, 0.5f, 0.3f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, separationRadius);

        if (_isStuck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.18f);
        }
    }
}
