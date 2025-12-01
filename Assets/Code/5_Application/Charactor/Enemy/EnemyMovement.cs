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
    public float avoidForce = 2f;
    public LayerMask obstacleMask;

    [Header("Separation")]
    public float separationRadius = 0.7f;
    public float separationForce = 1.5f;
    public LayerMask enemyMask;

    [Header("Debug")]
    public bool debugDraw = true;

    private Rigidbody2D _rb;
    public BoxCollider2D _box;
    private Vector2 _halfExtents;

    private Vector2 _smoothedDir;
    private bool _isDash = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _halfExtents = _box.size * 0.5f;
    }

    // =======================================================================
    // DASH / IMPULSE
    // =======================================================================

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

    // =======================================================================
    // MAIN UPDATE
    // =======================================================================

    public void MoveTowards(Vector2 desiredDirection)
    {
        if (_isDash)
            return;

        if (desiredDirection.sqrMagnitude < 0.001f)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        Vector2 forward = desiredDirection.normalized;

        Vector2 avoidVec = BoxAvoidance(forward);
        Vector2 separationVec = ComputeSeparation();
        Vector2 gapVec;
        bool foundGap = TryFindGap_Box(forward, out gapVec);

        Vector2 final =
            forward
            + avoidVec
            + separationVec * separationForce
            + (foundGap ? gapVec * 1.1f : Vector2.zero);

        if (final.sqrMagnitude > 1f)
            final.Normalize();

        _smoothedDir = Vector2.Lerp(_smoothedDir, final, Time.deltaTime * turnSharpness);

        _rb.velocity = _smoothedDir * speed;
    }

    public void Stop() => _rb.velocity = Vector2.zero;

    // =======================================================================
    // OBSTACLE AVOIDANCE (BoxCollider 4-edge rays)
    // =======================================================================

    private Vector2 BoxAvoidance(Vector2 forward)
    {
        Vector2 pos = transform.position;
        Vector2 perp = Vector2.Perpendicular(forward).normalized;
        float dist = avoidDistance;

        // 4 corners of box
        Vector2 lt = pos + new Vector2(-_halfExtents.x, +_halfExtents.y);
        Vector2 lb = pos + new Vector2(-_halfExtents.x, -_halfExtents.y);
        Vector2 rt = pos + new Vector2(+_halfExtents.x, +_halfExtents.y);
        Vector2 rb = pos + new Vector2(+_halfExtents.x, -_halfExtents.y);

        bool hitCenter = Physics2D.Raycast(pos, forward, dist, obstacleMask);
        bool hitLeft = Physics2D.Raycast(lt, forward, dist, obstacleMask) ||
                       Physics2D.Raycast(lb, forward, dist, obstacleMask);
        bool hitRight = Physics2D.Raycast(rt, forward, dist, obstacleMask) ||
                        Physics2D.Raycast(rb, forward, dist, obstacleMask);

        if (!hitCenter && !hitLeft && !hitRight)
            return Vector2.zero;

        Vector2 avoid = Vector2.zero;

        if (hitCenter)
        {
            if (!hitLeft) avoid += -perp;
            else if (!hitRight) avoid += perp;
            else avoid += Random.value > 0.5f ? perp : -perp;
        }
        else if (hitLeft && !hitRight)
            avoid += perp;
        else if (!hitLeft && hitRight)
            avoid += -perp;

        return avoid * avoidForce;
    }

    // =======================================================================
    // GAP NAVIGATION (find openings)
    // =======================================================================

    private bool TryFindGap_Box(Vector2 forward, out Vector2 gapVec)
    {
        gapVec = Vector2.zero;

        Vector2 perp = Vector2.Perpendicular(forward).normalized;
        Vector2 pos = transform.position;

        float checkDist = avoidDistance * 1.5f;

        bool leftBlocked =
            Physics2D.Raycast(pos - perp * _halfExtents.x, forward, checkDist, obstacleMask);

        bool rightBlocked =
            Physics2D.Raycast(pos + perp * _halfExtents.x, forward, checkDist, obstacleMask);

        if (!leftBlocked)
        {
            gapVec = -perp;
            return true;
        }
        if (!rightBlocked)
        {
            gapVec = perp;
            return true;
        }

        return false;
    }

    // =======================================================================
    // SEPARATION (repulsion force)
    // =======================================================================

    private Vector2 ComputeSeparation()
    {
        Vector2 pos = transform.position;

        Collider2D[] results = Physics2D.OverlapCircleAll(pos, separationRadius, enemyMask);
        Vector2 force = Vector2.zero;

        foreach (var col in results)
        {
            if (col.transform == transform) continue;

            Vector2 diff = pos - (Vector2)col.transform.position;
            float dist = diff.magnitude;
            if (dist < 0.001f) continue;

            float push = 1f - (dist / separationRadius);
            force += diff.normalized * push;
        }

        return force;
    }

    // =======================================================================
    // DEBUG GIZMOS
    // =======================================================================

    void OnDrawGizmos()
    {
        if (!debugDraw || _box == null) return;

        Vector2 pos = transform.position;

        // forward
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + _smoothedDir * 0.8f);

        // collider box
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireCube(pos + _box.offset, _box.size);

        // avoidance rays
        Vector2 forward = _smoothedDir.normalized;
        if (forward.sqrMagnitude < 0.1f) forward = Vector2.up;

        Vector2 lt = pos + new Vector2(-_halfExtents.x, +_halfExtents.y);
        Vector2 lb = pos + new Vector2(-_halfExtents.x, -_halfExtents.y);
        Vector2 rt = pos + new Vector2(+_halfExtents.x, +_halfExtents.y);
        Vector2 rb = pos + new Vector2(+_halfExtents.x, -_halfExtents.y);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(lt, lt + forward * avoidDistance);
        Gizmos.DrawLine(lb, lb + forward * avoidDistance);
        Gizmos.DrawLine(rt, rt + forward * avoidDistance);
        Gizmos.DrawLine(rb, rb + forward * avoidDistance);

        // separation radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, separationRadius);
    }
}
