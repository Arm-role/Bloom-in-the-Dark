using UnityEngine;

public class EnemyNavigationAgent
{
  private EnemyController owner;
  private Transform _target;

  private Vector3 _lastBuiltTargetPos;
  private const float REBUILD_THRESHOLD = 0.5f;

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
    var manager = FlowFieldManager.Instance;

    // ✅ ถ้า field มีอยู่แล้ว → ใช้เลย ไม่ rebuild
    // FlowFieldNavigationService จัดการ rebuild เมื่อ target ขยับ
    if (manager.TryGetField(flowTarget.FlowKey, footprint, out _))
      return;

    // ไม่มี field → สั่ง service build
    FlowFieldNavigationService.Instance.EnsureField(
        flowTarget.FlowKey, footprint, _target.position);
  }


  public bool HasReachedTarget(float padding = 0.1f)
  {
    if (_target == null) return true;

    var grid = FlowFieldManager.Instance.world.GridConverter;
    var fp = owner.FlowFieldOwner;
    float cellSize = grid.CellSize;

    // ระยะจาก pivot ไปถึง edge ที่ไกลที่สุด รองรับทศนิยม
    float pivotToEdge = Mathf.Max(
        Mathf.Max(fp.PivotAnchor.x, fp.Footprint.x - 1 - fp.PivotAnchor.x),
        Mathf.Max(fp.PivotAnchor.y, fp.Footprint.y - 1 - fp.PivotAnchor.y)
    ) * cellSize;

    float targetRadius = _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    float dist = Vector2.Distance(owner.transform.position, _target.position);

    return dist <= pivotToEdge + targetRadius + padding;
  }
}
