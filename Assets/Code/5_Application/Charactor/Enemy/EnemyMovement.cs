using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Core")]
    public float speed = 3.0f;
    public float accel = 12f;
    public float turnSharpness = 10f;

    [Header("FlowField")]
    public string flowKey = "AttackPlayer";

    [Header("Avoidance")]
    public float avoidDistance = 1.1f;
    public float avoidStrength = 1.0f;
    public LayerMask obstacleMask;

    [Header("Separation")]
    public float separationRadius = 0.8f;
    public float separationStrength = 1f;

    [Header("LOD (Level of Detail)")]
    public float LOD_Mid = 10f;
    public float LOD_Far = 18f;
    public int farSkip = 2;     // update every N frames

    [Header("Debug")]
    public bool debugDraw = false;

    Rigidbody2D rb;
    Collider2D col;

    Vector2 smoothedDir;
    Vector2 currentVelocity;
    bool isDash;

    RaycastHit2D[] ray = new RaycastHit2D[4];
    Collider2D[] cols = new Collider2D[12];
    int farCounter = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // =======================================================================================
    public void ManualFixedUpdate()
    {
        if (isDash) return;

        FlowFieldManager ff = FlowFieldManager.Instance;
        if (ff == null) { rb.velocity = Vector2.zero; return; }

        Vector2 flow = SampleFlow();
        if (flow == Vector2.zero)
        {
            // fallback → เดินตรงไปหา target แบบ safe
            Vector3 target = FlowFieldTarget();
            Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
            rb.velocity = dir * speed * 0.5f;
            return;
        }

        float distToTarget = Vector2.Distance(transform.position, FlowFieldTarget());

        // -----------------------------------------------------------------------------------
        // LOD Tier 3: Far (very cheap)
        // -----------------------------------------------------------------------------------
        if (distToTarget > LOD_Far)
        {
            farCounter++;
            if (farCounter % farSkip != 0)
                return; // skip this frame

            rb.velocity = flow * speed;  // full speed, no steering
            return;
        }

        // -----------------------------------------------------------------------------------
        // LOD Tier 2: Mid (moderate steering)
        // -----------------------------------------------------------------------------------
        if (distToTarget > LOD_Mid)
        {
            Vector2 desired = flow;

            smoothedDir = Vector2.Lerp(smoothedDir, desired, Time.deltaTime * turnSharpness);
            rb.velocity = smoothedDir * speed;
            return;
        }

        // -----------------------------------------------------------------------------------
        // LOD Tier 1: Near target (full AI steering)
        // -----------------------------------------------------------------------------------
        Vector2 avoid = ComputeObstacleAvoidance(flow) * avoidStrength;

        Vector2 sep = ComputeSeparation() * separationStrength;

        Vector2 steering = flow + avoid + sep;
        if (steering.sqrMagnitude > 1) steering.Normalize();

        // turn smoothing
        smoothedDir = Vector2.Lerp(smoothedDir, steering, Time.deltaTime * turnSharpness);

        // acceleration smoothing (no double lerp)
        currentVelocity = Vector2.MoveTowards(currentVelocity, smoothedDir * speed, accel * Time.deltaTime);

        rb.velocity = currentVelocity;

        if (debugDraw) DrawDebug(flow, avoid, sep, steering);
    }

    // =======================================================================================
    // FLOWFIELD UTILITIES
    // =======================================================================================

    private Vector2 SampleFlow()
    {
        var ff = FlowFieldManager.Instance;
        var grid = ff.world.GridConverter;

        Vector3 pos = transform.position;
        Vector3 cellF = grid.WorldToCell(pos); // ← (ต้องเพิ่มฟังก์ชันนี้ใน GridConverter)

        int x0 = Mathf.FloorToInt(cellF.x);
        int y0 = Mathf.FloorToInt(cellF.y);

        float tx = cellF.x - x0;
        float ty = cellF.y - y0;

        // sample 4 cells
        Vector2 d00 = ff.GetDirection(flowKey, new Vector3Int(x0, y0, 0));
        Vector2 d10 = ff.GetDirection(flowKey, new Vector3Int(x0 + 1, y0, 0));
        Vector2 d01 = ff.GetDirection(flowKey, new Vector3Int(x0, y0 + 1, 0));
        Vector2 d11 = ff.GetDirection(flowKey, new Vector3Int(x0 + 1, y0 + 1, 0));

        // bilinear blend
        Vector2 dx0 = Vector2.Lerp(d00, d10, tx);
        Vector2 dx1 = Vector2.Lerp(d01, d11, tx);
        Vector2 d = Vector2.Lerp(dx0, dx1, ty);

        return d.sqrMagnitude > 0.01f ? d.normalized : Vector2.zero;
    }

    private Vector3 FlowFieldTarget()
    {
        var ff = FlowFieldManager.Instance;
        FlowField field = ff.GetField(flowKey);
        return ff.world.GridConverter.CellToWorld(field.originCell);
    }

    // =======================================================================================
    // AVOIDANCE (AAA wall slide)
    // =======================================================================================

    private Vector2 ComputeObstacleAvoidance(Vector2 forward)
    {
        if (forward.sqrMagnitude < 0.01f) return Vector2.zero;

        Vector2 sum = Vector2.zero;
        int samples = 5;
        float spread = 45f;

        for (int i = 0; i < samples; i++)
        {
            float t = (i / (float)(samples - 1)) - 0.5f;
            float angle = t * spread;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, avoidDistance, obstacleMask);
            if (hit.collider != null)
            {
                Vector2 normal = hit.normal;
                sum += normal;
            }
        }

        return sum.sqrMagnitude > 0 ? sum.normalized : Vector2.zero;
    }

    // =======================================================================================
    // SEPARATION
    // =======================================================================================

    private Vector2 ComputeSeparation()
    {
        Vector2 sum = Vector2.zero;
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            separationRadius,
            cols,
            LayerMask.GetMask("Enemy")
        );

        int contributing = 0;

        for (int i = 0; i < count; i++)
        {
            if (cols[i].transform == transform) continue;

            Vector2 diff = (Vector2)(transform.position - cols[i].transform.position);
            float d = diff.magnitude;
            if (d < 0.05f) continue;              // deadzone กัน jitter
            if (d > separationRadius) continue;

            float strength = 1f - (d / separationRadius);

            sum += diff.normalized * strength;
            contributing++;
        }

        if (contributing == 0)
            return Vector2.zero;

        // normalize ท้ายสุดเพื่อลด jitter และแรงเกินจริง
        sum = sum.normalized;

        // ลด separation เมื่อใกล้เป้าหมายมาก (important!)
        float dist = Vector2.Distance(transform.position, FlowFieldTarget());
        float mul = Mathf.Clamp01(dist / 3f);   // 0 → 3 เมตร
        sum *= mul;

        return sum;
    }

    // =======================================================================================
    public void ApplyImpulse(Vector2 impulse, float duration)
    {
        if (isDash) return;
        isDash = true;

        rb.velocity = impulse;
        StartCoroutine(EndDash(duration));
    }

    IEnumerator EndDash(float t)
    {
        yield return new WaitForSeconds(t);
        isDash = false;
        currentVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    // =======================================================================================
    void DrawDebug(Vector2 flow, Vector2 avoid, Vector2 sep, Vector2 final)
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)flow * 0.7f, Color.green);
        Debug.DrawLine(transform.position, transform.position + (Vector3)avoid * 0.7f, Color.red);
        Debug.DrawLine(transform.position, transform.position + (Vector3)sep * 0.7f, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + (Vector3)final * 0.7f, Color.cyan);
    }
}
