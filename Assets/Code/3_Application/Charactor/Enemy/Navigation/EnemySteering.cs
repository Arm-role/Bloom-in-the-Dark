using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySteering : MonoBehaviour
{
  [SerializeField]
  private SteeringProfileSO profile;

  public FlowFieldChannelKey flowKey { get; set; }

  public float moveSpeed;
  public float turnSpeed;
  public float accel;

  public float obstacleDist;
  public float obstacleStrength;
  public float avoidAngle;
  public LayerMask obstacleMask;

  public float separationRadius;
  public float separationStrength;
  public LayerMask enemyLayerMask;

  public float cornerRadius;
  public float cornerPush;
  public float passageProbeDist;
  public float narrowThreshold;
  public float centerStrength;
  public float narrowSpeedMul;

  // internal
  private Collider2D col;
  private float agentRadius;
  private Collider2D[] nearby = new Collider2D[16];
  private Vector2 lastFlow = Vector2.zero;

  public Vector2? ForcedDirection { get; internal set; }

  void Awake()
  {
    col = GetComponent<Collider2D>();
    agentRadius = col != null ? col.bounds.extents.x : 0.4f;

    ApplyProfile();
  }

  void ApplyProfile()
  {
    moveSpeed = profile.moveSpeed;
    turnSpeed = profile.turnSpeed;
  }

  public SteeringResult TickSteering()
  {
    var ff = FlowFieldManager.Instance;

    if (ff == null || !ff.TryGetField(flowKey, out var f))
      return SteeringResult.Zero;

    if (ForcedDirection.HasValue)
    {
      var forced = ForcedDirection.Value;
      ForcedDirection = null;
      return new SteeringResult(forced.normalized, 1f, 5f);
    }

    Debug.Log($"FlowKey = {flowKey}");
    Debug.Log($"FlowField exist: {FlowFieldManager.Instance.TryGetField(flowKey, out _)}");

    Vector2 flow = SampleFlow(ff);
    lastFlow = flow;


    Debug.DrawRay(transform.position, flow * 2f, Color.green);

    if (flow.sqrMagnitude < 0.001f)
    {
      Debug.LogWarning(
        $"{name} flow = ZERO"
      );

      return SteeringResult.Zero;
    }

    Vector2 steer = flow;

    steer += DetectCorner();
    bool inNarrow = DetectNarrowPassage(flow, out Vector2 centerOffset, out float measuredWidth);

    if (inNarrow)
    {
      steer += CenteringForce(centerOffset) * centerStrength;
      steer += AvoidObstacle(flow) * 0.5f;
    }
    else
    {
      steer += AvoidObstacle(flow);
    }

    steer += Separation();

    if (steer.sqrMagnitude > 1f) steer.Normalize();

    if (flow.sqrMagnitude < 0.01f)
      steer += EscapeFromObstacle();

    float speedMul = inNarrow ? narrowSpeedMul : 1f;

    var field = ff.GetField(flowKey);

    if (field != null)
    {
      Vector3 origin =
          ff.world.GridConverter.CellToWorld(
              field.originCell
          );

      Debug.DrawLine(
          transform.position,
          origin,
          Color.magenta
      );
    }

    Debug.DrawRay(
    transform.position,
    steer * 2f,
    Color.white
    );

    return new SteeringResult(steer.normalized, speedMul, 1f);
  }

  // ------------------------
  // Sampling (bilinear) - uses GridConverter - supports float/int via reflection fallback
  // ------------------------
  Vector2 SampleFlow(FlowFieldManager ff)
  {
    var field = ff.GetField(flowKey);
    if (field == null) return Vector2.zero;

    var gridConv = ff.world.GridConverter;

    Vector3 cellF = GetCellFloat(gridConv, transform.position);

    // 🔥 world → local field
    cellF.x -= field.originCell.x;
    cellF.y -= field.originCell.y;

    int x = Mathf.FloorToInt(cellF.x);
    int y = Mathf.FloorToInt(cellF.y);

    float tx = cellF.x - x;
    float ty = cellF.y - y;

    // 🔥 CLAMP (ตัวแก้ bug จริง)
    int maxX = field.width - 1;
    int maxY = field.height - 1;

    int x0 = Mathf.Clamp(x, 0, maxX);
    int y0 = Mathf.Clamp(y, 0, maxY);
    int x1 = Mathf.Clamp(x + 1, 0, maxX);
    int y1 = Mathf.Clamp(y + 1, 0, maxY);

    Vector2 d00 = field.GetDirection(new Vector2Int(x0, y0));
    Vector2 d10 = field.GetDirection(new Vector2Int(x1, y0));
    Vector2 d01 = field.GetDirection(new Vector2Int(x0, y1));
    Vector2 d11 = field.GetDirection(new Vector2Int(x1, y1));

    Vector2 dx0 = Vector2.Lerp(d00, d10, tx);
    Vector2 dx1 = Vector2.Lerp(d01, d11, tx);
    Vector2 d = Vector2.Lerp(dx0, dx1, ty);

    Vector3 cellWorld =
      ff.world.GridConverter.CellToWorld(
          new Vector3Int(x, y, 0)
      );

    Debug.DrawLine(
      transform.position,
      cellWorld,
      Color.yellow
    );

    Debug.Log(
      $"agent cell = {x},{y} | origin = {field.originCell}"
    );

    return d.sqrMagnitude > 0.0001f ? d.normalized : Vector2.zero;
  }

  Vector3 GetCellFloat(object conv, Vector3 worldPos)
  {
    if (conv == null) return worldPos;
    var t = conv.GetType();

    var mi = t.GetMethod("WorldToCell",
      System.Reflection.BindingFlags.Instance |
      System.Reflection.BindingFlags.Public |
      System.Reflection.BindingFlags.NonPublic);

    if (mi != null)
    {
      object res = mi.Invoke(conv, new object[] { worldPos });
      if (res is Vector3 vf) return vf;
      if (res is Vector3Int vi) return new Vector3(vi.x, vi.y, vi.z);
    }

    var mi2 = t.GetMethod("WorldToCellFloat",
      System.Reflection.BindingFlags.Instance |
      System.Reflection.BindingFlags.Public |
      System.Reflection.BindingFlags.NonPublic);

    if (mi2 != null)
    {
      object res = mi2.Invoke(conv, new object[] { worldPos });
      if (res is Vector3 vf) return vf;
    }

    var mi3 = t.GetMethod("WorldToCell", new Type[] { typeof(Vector3) });

    if (mi3 != null)
    {
      object res = mi3.Invoke(conv, new object[] { worldPos });
      if (res is Vector3Int vi) return new Vector3(vi.x, vi.y, 0f);
    }

    return worldPos;
  }

  // ------------------------
  // Corner detection
  // ------------------------
  Vector2 DetectCorner()
  {
    Vector2 push = Vector2.zero;
    float probeR = agentRadius + 0.05f;
    for (int i = 0; i < 8; i++)
    {
      float a = i * 45f;
      Vector2 dir = Quaternion.Euler(0, 0, a) * Vector2.right;
      Vector2 samplePos = (Vector2)transform.position + dir * probeR;
      if (Physics2D.OverlapCircle(samplePos, agentRadius * 0.5f, obstacleMask))
        push += -dir;
    }
    if (push.sqrMagnitude < 0.001f) return Vector2.zero;
    return push.normalized * cornerPush;
  }

  // ------------------------
  // Obstacle avoidance (multi-ray)
  // ------------------------
  Vector2 AvoidObstacle(Vector2 forward)
  {
    if (forward.sqrMagnitude < 0.1f) return Vector2.zero;
    Vector2 sum = Vector2.zero;
    int samples = 5;
    for (int i = 0; i < samples; i++)
    {
      float t = samples == 1 ? 0f : (i / (float)(samples - 1)) - 0.5f;
      float angle = t * avoidAngle;
      Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;
      RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        dir,
        obstacleDist, obstacleMask);

      if (hit.collider != null)
        sum += hit.normal;
    }

    Debug.DrawRay(
    transform.position,
    sum,
    Color.red
    );

    return sum.sqrMagnitude > 0.001f ? sum.normalized * obstacleStrength : Vector2.zero;
  }

  // ------------------------
  // Separation (boids)
  // ------------------------
  Vector2 Separation()
  {
    int count = Physics2D.OverlapCircleNonAlloc(
      transform.position,
      separationRadius,
      nearby,
      enemyLayerMask
    );

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

    Debug.DrawRay(
    transform.position,
    sum,
    Color.blue
    );

    if (c == 0) return Vector2.zero;
    return sum.normalized * separationStrength;
  }

  // ------------------------
  // Narrow passage detection + centering
  // ------------------------
  bool DetectNarrowPassage(Vector2 forward, out Vector2 centerOffset, out float measuredWidth)
  {
    centerOffset = Vector2.zero;
    measuredWidth = 100f;

    if (forward.sqrMagnitude < 0.01f) return false;

    Vector2 f = forward.normalized;
    Vector2 perp = Vector2.Perpendicular(f).normalized;
    Vector2 probeCenter = (Vector2)transform.position + f * passageProbeDist;

    float maxCheck = 1.5f;
    int steps = 8;
    float leftHit = maxCheck, rightHit = maxCheck;

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

    measuredWidth = leftHit + rightHit;
    float signed = Vector2.Dot((Vector2)transform.position - probeCenter, perp);
    centerOffset = perp * signed;

    return measuredWidth < narrowThreshold;
  }

  Vector2 CenteringForce(Vector2 centerOffset)
  {
    if (centerOffset.sqrMagnitude < 0.001f) return Vector2.zero;
    float mag = centerOffset.magnitude;
    Vector2 dir = -centerOffset.normalized;

    return dir * Mathf.Clamp01(mag / 1.0f);
  }

  Vector2 EscapeFromObstacle()
  {
    RaycastHit2D hit = Physics2D.CircleCast(
      transform.position,
      0.25f,
      Vector2.zero,
      0,
      obstacleMask);

    if (!hit) return Vector2.zero;
    return hit.normal.normalized * 1.2f;
  }

#if UNITY_EDITOR
  void OnDrawGizmos()
  {
    if (!Application.isPlaying)
      return;

    Gizmos.color = Color.green;

    Gizmos.DrawLine(
        transform.position,
        transform.position +
        (Vector3)lastFlow * 1.5f
    );

    Gizmos.DrawSphere(
        transform.position +
        (Vector3)lastFlow * 1.5f,
        0.08f
    );
  }
#endif
}
