using System;
using System.Collections.Generic;
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

  private float _obstacleCheckTimer;
  private float _obstacleCheckInterval = 0.1f; // 10Hz แทน 50Hz

  private Vector2 _cachedObstacleForce;
  private Vector2 _cachedSeparationForce;
  private bool _cachedInNarrow;
  private Vector2 _cachedCenterOffset;

  // internal
  private Collider2D col;
  private float agentRadius;
  private Collider2D[] nearby = new Collider2D[16];
  private Vector2 lastFlow = Vector2.zero;

  private static readonly List<Vector3Int> _footprintBuffer = new List<Vector3Int>();
  private static readonly Vector3Int[] _sampleOffsets = {
      Vector3Int.zero,
      Vector3Int.up, Vector3Int.down,
      Vector3Int.left, Vector3Int.right,
  };
  public Vector2? ForcedDirection { get; private set; }
  private FlowFieldOwner _owner;
  void Awake()
  {
    col = GetComponent<Collider2D>();
    agentRadius = col != null ? col.bounds.extents.x : 0.4f;
    _owner = GetComponent<FlowFieldOwner>();
    ApplyProfile();
  }

  void OnEnable()
  {
    // กระจาย check ให้ไม่ตรงกัน
    _obstacleCheckTimer = UnityEngine.Random.Range(0f, _obstacleCheckInterval);
  }

  void ApplyProfile()
  {
    moveSpeed = profile.moveSpeed;
    turnSpeed = profile.turnSpeed;
  }

  public SteeringResult TickSteering(Vector2Int footprint)
  {
    var ff = FlowFieldManager.Instance;

    if (ff == null || !ff.TryGetField(flowKey, footprint, out var f))
      return SteeringResult.Zero;

    if (ForcedDirection.HasValue)
    {
      var forced = ForcedDirection.Value;
      ForcedDirection = null;
      return new SteeringResult(forced.normalized, 1f, 5f);
    }

    Vector2 flow = SampleFlow(footprint, ff);

    //Debug.DrawRay(transform.position, flow * 2f, Color.green);

    // BUG FIX: ถ้า footprint sample ทั้งหมด zero → fallback sample จาก pivot เดียว
    if (flow.sqrMagnitude < 0.001f)
    {
      flow = SampleFlowSingleCell(footprint, ff);
    }

    lastFlow = flow;

    if (flow.sqrMagnitude < 0.001f)
      return SteeringResult.Zero;

    _obstacleCheckTimer -= Time.fixedDeltaTime;
    if (_obstacleCheckTimer <= 0f)
    {
      _obstacleCheckTimer = _obstacleCheckInterval;

      _cachedObstacleForce = DetectCorner();
      _cachedObstacleForce += AvoidObstacle(flow);
      _cachedSeparationForce = Separation();
      _cachedInNarrow = DetectNarrowPassage(
          flow,
          out _cachedCenterOffset,
          out _
      );
    }

    Vector2 steer = flow;
    steer += _cachedObstacleForce;
    steer += _cachedSeparationForce;

    if (_cachedInNarrow)
      steer += CenteringForce(_cachedCenterOffset) * centerStrength;

    if (steer.sqrMagnitude > 1f) steer.Normalize();

    float speedMul = _cachedInNarrow ? narrowSpeedMul : 1f;

    return new SteeringResult(steer.normalized, speedMul, 1f);
  }

  public bool HasDirection(Vector2Int footprint)
  {
    var ff = FlowFieldManager.Instance;

    if (!ff.TryGetField(flowKey, footprint, out var field))
      return false;

    var grid = ff.world.GridConverter;

    Vector3Int cell =
      grid.WorldToCell(transform.position);

    Vector2Int local =
      new Vector2Int(
        cell.x - field.originCell.x,
        cell.y - field.originCell.y
      );

    if (!field.IsInside(local))
      return false;

    return field.GetDirection(local) != Vector2.zero;
  }

  // ------------------------
  // Sampling (bilinear) - uses GridConverter - supports float/int via reflection fallback
  // ------------------------
  Vector2 SampleFlow(Vector2Int footprint, FlowFieldManager ff)
  {
    var field = ff.GetField(flowKey, footprint);
    if (field == null) return Vector2.zero;

    var grid = ff.world.GridConverter;
    _owner.GetFootprintCells(transform.position, grid, _footprintBuffer);

    Vector2 sum = Vector2.zero;
    int samples = 0;

    foreach (var cell in _footprintBuffer)
    {
      Vector2Int local = new Vector2Int(
          cell.x - field.originCell.x,
          cell.y - field.originCell.y
      );

      if (!field.IsInside(local)) continue;

      // ✅ ข้าม impassable แต่ไม่ return zero — ยังเก็บ cell อื่นไว้
      if (field.GetCost(local) == FlowField.COST_IMPASSABLE) continue;

      Vector2 dir = field.GetDirection(local);
      if (dir == Vector2.zero) continue;

      sum += dir;
      samples++;
    }

    if (samples == 0) return Vector2.zero;
    return sum.normalized;
  }

  Vector2 SampleFlowSingleCell(Vector2Int footprint, FlowFieldManager ff)
  {
    var field = ff.GetField(flowKey, footprint);
    if (field == null) return Vector2.zero;

    var grid = ff.world.GridConverter;

    // ลอง sample รอบๆ ในรัศมี 1 cell หา cell ที่เดินได้
    Vector3Int pivotCell = grid.WorldToCell(transform.position);

    foreach (var offset in _sampleOffsets)
    {
      var cell = pivotCell + offset;
      Vector2Int local = new Vector2Int(
          cell.x - field.originCell.x,
          cell.y - field.originCell.y
      );

      if (!field.IsInside(local)) continue;
      if (field.GetCost(local) == FlowField.COST_IMPASSABLE) continue;

      Vector2 dir = field.GetDirection(local);
      if (dir != Vector2.zero) return dir;
    }

    return Vector2.zero;
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
