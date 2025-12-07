using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFlowSteering : MonoBehaviour
{
    public string flowKey = "AttackPlayer";

    [Header("Speed")]
    public float moveSpeed = 3f;
    public float turnSpeed = 10f;
    public float accel = 14f;
    [Tooltip("Max speed multiplier while in narrow passage (0..1)")]
    public float narrowSpeedMul = 0.6f;

    [Header("Avoidance")]
    public float obstacleDist = 0.9f;
    public float obstacleStrength = 1.4f;
    public float avoidAngle = 40f;
    public LayerMask obstacleMask;

    [Header("Agent Separation")]
    public float separationRadius = 0.9f;
    public float separationStrength = 1.2f;

    [Header("Corner / Narrow Passage")]
    public float cornerRadius = 0.55f;
    [Tooltip("how strongly we push away from corner")]
    public float cornerPush = 1.8f;
    [Tooltip("how far ahead (world) to probe for passage width")]
    public float passageProbeDist = 1.2f;
    [Tooltip("minimum passage width (world units) considered 'narrow'")]
    public float narrowThreshold = 0.95f; // ~1 tile
    [Tooltip("how strongly to center inside passage")]
    public float centerStrength = 1.6f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 smoothedDir;
    private Vector2 velocity;
    private Collider2D[] nearby = new Collider2D[16];

    // cached
    private float agentRadius;
    private Vector2 lastFlow = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        // estimate agent radius from collider bounds (assume roughly circular)
        agentRadius = col.bounds.extents.x;
    }

    public void Tick()
    {
        FlowFieldManager ff = FlowFieldManager.Instance;
        if (ff == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // --------------------------
        // Flow sample (bilinear)
        // --------------------------
        Vector2 flow = SampleFlow(ff);
        lastFlow = flow;

        // If no flow then try safe fallback (towards target)
        if (flow.sqrMagnitude < 0.001f)
        {
            // fallback: small direct steer to target (safe, slower)
            Vector3 fieldTarget = ff.GetField(flowKey)?.IndexToWorld(0, 0, ff.world.GridConverter) ?? transform.position;
            Vector2 fallback = (fieldTarget - transform.position);
            if (fallback.sqrMagnitude < 0.001f) { rb.velocity = Vector2.zero; return; }
            flow = fallback.normalized * 0.4f; // gentle
        }

        // --------------------------
        // Narrow passage detection
        // --------------------------
        bool inNarrow = DetectNarrowPassage(flow, out Vector2 passageCenterOffset, out float passageWidth);

        // --------------------------
        // Steering composition
        // --------------------------
        Vector2 steer = flow;

        // Corner repulsion (improved)
        steer += DetectCorner();

        // If in narrow passage, add centering force and reduce speed
        if (inNarrow)
        {
            // centerStrength scales how strongly to pull toward centerline
            steer += CenteringForce(passageCenterOffset) * centerStrength;

            // mild obstacle avoidance still useful
            steer += AvoidObstacle(flow) * 0.5f;
        }
        else
        {
            // normal obstacle avoidance
            steer += AvoidObstacle(flow);
        }

        // agent separation (always)
        steer += Separation();

        // Limit and finalize
        if (steer.sqrMagnitude > 1f) steer.Normalize();

        // smooth turn
        smoothedDir = Vector2.Lerp(smoothedDir, steer, Time.deltaTime * turnSpeed);

        // speed multiplier in narrow
        float targetSpeed = moveSpeed * (inNarrow ? narrowSpeedMul : 1f);

        // accelerate
        velocity = Vector2.MoveTowards(velocity, smoothedDir * targetSpeed, accel * Time.deltaTime);
        rb.velocity = velocity;
    }

    // ===================================================================
    // FLOW SAMPLE (bilinear)
    // ===================================================================
    Vector2 SampleFlow(FlowFieldManager ff)
    {
        var grid = ff.world.GridConverter;

        Vector3 worldPos = transform.position;
        Vector3 cellF = grid.WorldToCell(worldPos); // uses Tilemap.WorldToCell returning Vector3Int, but we expect float-support
        // If your GridConverter returns Vector3Int only, you should implement WorldToCellFloat to get fractional cell coords.
        float fx = cellF.x;
        float fy = cellF.y;

        int x = Mathf.FloorToInt(fx);
        int y = Mathf.FloorToInt(fy);

        float tx = fx - x;
        float ty = fy - y;

        Vector2 d00 = ff.GetDirection(flowKey, new Vector3Int(x, y, 0));
        Vector2 d10 = ff.GetDirection(flowKey, new Vector3Int(x + 1, y, 0));
        Vector2 d01 = ff.GetDirection(flowKey, new Vector3Int(x, y + 1, 0));
        Vector2 d11 = ff.GetDirection(flowKey, new Vector3Int(x + 1, y + 1, 0));

        Vector2 d0 = Vector2.Lerp(d00, d10, tx);
        Vector2 d1 = Vector2.Lerp(d01, d11, tx);
        Vector2 f = Vector2.Lerp(d0, d1, ty);

        return f.sqrMagnitude < 0.001f ? Vector2.zero : f.normalized;
    }

    // ===================================================================
    // IMPROVED CORNER REPULSION
    // ===================================================================
    Vector2 DetectCorner()
    {
        Vector2 push = Vector2.zero;

        // sample 8 directions around - use agent radius to compute probe positions
        float probeR = agentRadius + 0.05f;
        for (int i = 0; i < 8; i++)
        {
            float a = i * 45f;
            Vector2 dir = Quaternion.Euler(0, 0, a) * Vector2.right;
            Vector2 samplePos = (Vector2)transform.position + dir * probeR;
            if (Physics2D.OverlapCircle(samplePos, agentRadius * 0.5f, obstacleMask))
            {
                // we are near an obstacle at that corner -> push away from that direction
                push += -dir;
            }
        }

        if (push.sqrMagnitude < 0.001f) return Vector2.zero;
        return push.normalized * cornerPush;
    }

    // ===================================================================
    // OBSTACLE AVOIDANCE (steer away from walls - multiple raycasts)
    // ===================================================================
    Vector2 AvoidObstacle(Vector2 forward)
    {
        if (forward.sqrMagnitude < 0.1f)
            return Vector2.zero;

        Vector2 sum = Vector2.zero;

        int samples = 5;
        for (int i = 0; i < samples; i++)
        {
            float t = samples == 1 ? 0f : (i / (float)(samples - 1)) - 0.5f;
            float angle = t * avoidAngle;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleDist, obstacleMask);
            if (hit.collider != null)
            {
                // steering away: along hit normal
                sum += hit.normal;
            }
        }
        if (sum.sqrMagnitude < 0.001f) return Vector2.zero;
        return sum.normalized * obstacleStrength;
    }

    // ===================================================================
    // SEPARATION
    // ===================================================================
    Vector2 Separation()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, separationRadius, nearby, LayerMask.GetMask("Enemy"));
        Vector2 sum = Vector2.zero;
        int c = 0;

        for (int i = 0; i < count; i++)
        {
            if (nearby[i].transform == transform) continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)nearby[i].transform.position;
            float d = diff.magnitude;

            if (d < 0.001f || d > separationRadius) continue;

            float w = 1f - (d / separationRadius);
            sum += diff.normalized * w;
            c++;
        }
        if (c == 0) return Vector2.zero;

        return sum.normalized * separationStrength;
    }

    // ===================================================================
    // NARROW PASSAGE DETECTION + CENTERING
    //   - returns true if narrow; also out: offset to centerline (world-space),
    //     and measured width (world units)
    // ===================================================================
    bool DetectNarrowPassage(Vector2 forward, out Vector2 centerOffset, out float measuredWidth)
    {
        centerOffset = Vector2.zero;
        measuredWidth = 100f;

        if (forward.sqrMagnitude < 0.01f) return false;

        Vector2 f = forward.normalized;
        Vector2 perp = Vector2.Perpendicular(f).normalized;

        // probe ahead a bit (passageProbeDist) and measure free width along perp
        Vector2 probeCenter = (Vector2)transform.position + f * passageProbeDist;

        // step along perp both sides until obstacle (raycasts)
        float maxCheck = 1.5f; // max half-width to check (world units)
        int steps = 8;
        float leftHit = maxCheck;
        float rightHit = maxCheck;

        for (int i = 1; i <= steps; i++)
        {
            float t = (i / (float)steps) * maxCheck;
            Vector2 pL = probeCenter + perp * t;
            Vector2 pR = probeCenter - perp * t;

            bool hitL = Physics2D.OverlapCircle(pL, agentRadius * 0.5f, obstacleMask);
            bool hitR = Physics2D.OverlapCircle(pR, agentRadius * 0.5f, obstacleMask);

            if (hitL && leftHit == maxCheck) leftHit = t;
            if (hitR && rightHit == maxCheck) rightHit = t;
        }

        // measured full width approx = leftHit + rightHit
        measuredWidth = leftHit + rightHit;

        // center offset: how far agent is from centerline (positive = to right)
        // compute signed distance from probe center to agent projected on perp
        float signed = Vector2.Dot((Vector2)transform.position - probeCenter, perp);
        centerOffset = perp * signed;

        // it's narrow if measuredWidth < narrowThreshold + some margin
        bool isNarrow = measuredWidth < narrowThreshold;
        return isNarrow;
    }

    // centering force: positive offset -> push towards negative (center)
    Vector2 CenteringForce(Vector2 centerOffset)
    {
        if (centerOffset.sqrMagnitude < 0.001f) return Vector2.zero;
        // target is 0 offset, so force = -offset normalized * amount scaled by magnitude
        float mag = centerOffset.magnitude;
        Vector2 dir = -centerOffset.normalized;
        // stronger if larger offset (clamp)
        return dir * Mathf.Clamp01(mag / 1.0f);
    }
}
