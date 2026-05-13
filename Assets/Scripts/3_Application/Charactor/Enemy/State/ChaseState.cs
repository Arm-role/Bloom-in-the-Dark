using UnityEngine;

public class ChaseState : IEnemyState
{
  private readonly EnemyController _c;

  private float _repathTimer;
  private const float REPTH_INTERVAL = 0.25f;

  private Transform _cachedTarget;
  private float _cachedTargetRadius;

  public ChaseState(EnemyController c)
  {
    _c = c;
  }

  public void Enter()
  {
    _repathTimer = 0f;
  }

  public void Exit()
  {
  }

  private float GetTargetRadius()
  {
    var t = _c.CurrentTarget;
    if (t != _cachedTarget)
    {
      _cachedTarget = t;
      _cachedTargetRadius = t?.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    }
    return _cachedTargetRadius;
  }

  public void ManualUpdate()
  {
    if (_c.CurrentTarget == null) { _c.ChangeState(_c.IdleState); return; }

    float ownerRadius = _c.CombatRadius;
    float targetRadius = GetTargetRadius();
    float dist = CombatDistanceUtility.EdgeDistance(
        _c.transform, ownerRadius,
        _c.CurrentTarget, targetRadius);

    if (_c.Combat.AnySkillReadyInRange(dist))
    {
      _c.ChangeState(_c.AttackState);
      return;
    }

    _repathTimer -= Time.deltaTime;
    if (_repathTimer <= 0f)
    {
      _repathTimer = REPTH_INTERVAL;
      _c.NavigationAgent.RequestFlowUpdateImmediate();
    }
  }

  public void ManualFixedUpdate()
  {
    // movement handled by steering
  }
}
