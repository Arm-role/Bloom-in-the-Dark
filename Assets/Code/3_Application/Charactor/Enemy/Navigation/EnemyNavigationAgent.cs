using UnityEngine;

public class EnemyNavigationAgent
{
  private EnemyController owner;
  private Transform _target;

  public EnemyNavigationAgent(
      EnemyController controller)
  {
    owner = controller;
  }

  public bool HasValidFlow =>
      owner.Steering.flowKey != null
      &&
      FlowFieldManager.Instance
          .TryGetField(
              owner.Steering.flowKey,
              owner.FlowFieldOwner.Footprint,
              out _
          );

  public void SetTarget(Transform target)
  {
    _target = target;

    if (_target == null)
      return;

    var flowTarget = _target.GetComponent<FlowFieldTarget>();

    if (flowTarget == null)
    {
      Debug.LogWarning(
          $"Target {_target.gameObject} missing FlowFieldTarget"
      );
      return;
    }

    if (owner.Steering.flowKey == flowTarget.FlowKey)
      return;

    owner.Steering.flowKey = flowTarget.FlowKey;

    RequestFlowUpdateImmediate();
  }

  public void RequestFlowUpdateImmediate()
  {
    if (_target == null) return;
    if (HasReachedTarget()) return;

    var flowTarget = _target.GetComponent<FlowFieldTarget>();
    if (flowTarget == null) return;

    var footprint = owner.FlowFieldOwner.Footprint;
    var pivotOffset = owner.FlowFieldOwner.PivotOffset;
    var manager = FlowFieldManager.Instance;

    manager.RemoveField(flowTarget.FlowKey, footprint);

    // ─── ตรวจก่อนว่า target cell เองเดินได้ไหม ───
    var grid = manager.world.GridConverter;
    Vector3Int targetCell = grid.WorldToCell(_target.position);

    bool targetReachable = manager.HasClearance(
        targetCell, footprint, pivotOffset
    );

    if (targetReachable)
    {
      manager.BuildField(flowTarget.FlowKey, footprint, _target.position);
    }
    else
    {
      // target อยู่ที่ที่ footprint ของเราเข้าไม่ได้ → หาจุดใกล้สุดที่เข้าได้
      var fallbackCells = manager.FindClosestReachableCells(
          _target.position, footprint, pivotOffset
      );

      if (fallbackCells.Count > 0)
      {
        manager.BuildField(flowTarget.FlowKey, footprint, fallbackCells);
      }
      else
      {
        // ไม่มีจุดไหนเข้าได้เลย → ปล่อยให้ agent ยืนอยู่กับที่ (HasValidFlow = false)
        Debug.LogWarning($"[Navigation] No reachable fallback for {_target.name}");
      }
    }

    // ─── ตรวจ agent เองว่าติดหรือเปล่า (เหมือนเดิม) ───
    var field = manager.GetField(flowTarget.FlowKey, footprint);
    if (field == null) return;

    Vector3Int agentCell = grid.WorldToCell(owner.transform.position);
    var rel = new Vector2Int(
        agentCell.x - field.originCell.x,
        agentCell.y - field.originCell.y
    );

    bool agentIsStranded =
        !field.IsInside(rel) ||
        field.GetIntegration(rel) >= int.MaxValue / 4;

    if (agentIsStranded)
    {
      var agentFallback = manager.FindClosestReachableCells(
          owner.transform.position, footprint, pivotOffset
      );

      // ถ้า fallback ของ agent ดีกว่า → rebuild ไปยัง fallback นั้น
      if (agentFallback.Count > 0)
      {
        manager.RemoveField(flowTarget.FlowKey, footprint);
        manager.BuildField(flowTarget.FlowKey, footprint, agentFallback);
      }
    }
  }


  public bool HasReachedTarget(float padding = 0.6f) // เพิ่ม padding จาก 0.1 → 0.6
  {
    if (_target == null) return true;

    var grid = FlowFieldManager.Instance.world.GridConverter;
    var footprint = owner.FlowFieldOwner.Footprint;
    var pivotOffset = owner.FlowFieldOwner.PivotOffset;
    float cellSize = grid.CellSize;

    // ระยะจาก pivot ถึง edge ไกลสุดของ footprint + ครึ่ง cell (ถึง edge จริง)
    float pivotToEdge = (Mathf.Max(
        Mathf.Max(pivotOffset.x, footprint.x - 1 - pivotOffset.x),
        Mathf.Max(pivotOffset.y, footprint.y - 1 - pivotOffset.y)
    ) + 0.5f) * cellSize;

    float targetRadius = _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;

    float dist = Vector2.Distance(owner.transform.position, _target.position);
    float stopDist = pivotToEdge + targetRadius + padding;

    return dist <= stopDist;
  }
}
