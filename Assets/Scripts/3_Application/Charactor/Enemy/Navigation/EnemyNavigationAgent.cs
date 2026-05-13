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

    if (_target == null) return;

    var flowTarget = _target.GetComponent<FlowFieldTarget>();

    if (flowTarget == null)
      return;

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

    FlowFieldNavigationService.Instance.EnsureField(
       flowTarget.FlowKey,
       owner.FlowFieldOwner.Footprint,
       _target.position);
  }


  public bool HasReachedTarget(float padding = 0.1f)
  {
    if (_target == null) return true;

    var grid = FlowFieldManager.Instance.world.GridConverter;
    var fp = owner.FlowFieldOwner;
    float cellSize = grid.CellSize;

    float pivotToEdge = Mathf.Max(
        Mathf.Max(fp.PivotAnchor.x, fp.Footprint.x - 1 - fp.PivotAnchor.x),
        Mathf.Max(fp.PivotAnchor.y, fp.Footprint.y - 1 - fp.PivotAnchor.y)
    ) * cellSize;

    float targetRadius = _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    float dist = Vector2.Distance(owner.transform.position, _target.position);

    return dist <= pivotToEdge + targetRadius + padding;
  }
}
