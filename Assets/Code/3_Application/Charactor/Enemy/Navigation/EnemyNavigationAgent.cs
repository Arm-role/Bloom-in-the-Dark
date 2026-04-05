using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
              out _
          );

  public void SetTarget(Transform target)
  {
    _target = target;

    if (_target == null)
      return;

    var flowTarget =
        _target.GetComponent<FlowFieldTarget>();

    if (flowTarget == null)
    {
      Debug.LogWarning(
          $"Target {_target.gameObject} missing FlowFieldTarget"
      );
      return;
    }

    if (owner.Steering.flowKey ==
        flowTarget.FlowKey)
      return;

    owner.Steering.flowKey =
        flowTarget.FlowKey;

    RequestFlowUpdateImmediate();
  }

  public void RequestFlowUpdateImmediate()
  {
    if (_target == null)
      return;

    var flowTarget =
        _target.GetComponent<FlowFieldTarget>();

    if (flowTarget == null)
      return;

    var targets = flowTarget.GetTargetCells();

    if (targets.Count() > 1)
    {
      FlowFieldManager
      .Instance
      .BuildField(
          flowTarget.FlowKey,
          targets
      );
    }
    else
    {
      FlowFieldNavigationService.Instance
      .EnsureField(
          flowTarget.FlowKey,
          _target.position
      );
    }

  }

  public bool HasReachedTarget(float padding = 0.1f)
  {
    if (_target == null)
      return true;

    float ownerRadius =
        owner.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;

    float targetRadius =
        _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;

    float dist =
        Vector2.Distance(
            owner.transform.position,
            _target.position
        );

    float stopDist =
        ownerRadius +
        targetRadius +
        padding;

    return dist <= stopDist;
  }
}
