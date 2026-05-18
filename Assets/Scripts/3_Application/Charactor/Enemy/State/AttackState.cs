using UnityEngine;

public class AttackState : IEnemyState
{
  private EnemyController _c;
  private EnemyPatternBrain _brain;

  private Transform _cachedTarget;
  private float _cachedTargetRadius;

  public AttackState(EnemyController c)
  {
    _c = c;
    _brain = c.PatternBrain;
  }

  public void Enter()
  {
    _brain?.StopPattern();
    if (_c.CurrentTarget != null)
      _brain?.Tick(_c);
  }

  public void ManualUpdate()
  {
    if(_c.CurrentTarget == null) { _c.ChangeState(_c.ChaseState); return; }

    float dist = EdgeDist();

    if (_brain == null) return;

    if (!_brain.IsRunning)
    {
      if (_c.Combat.AnySkillReadyInRange(dist))
        _brain.Tick(_c);
      else
        _c.ChangeState(_c.ChaseState);
    }
  }
  public void Exit()
  {
    _brain?.StopPattern();
  }
  public void ManualFixedUpdate() { }

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

  private float EdgeDist()
  {
    if (_c.CurrentTarget == null) return float.MaxValue;
    return CombatDistanceUtility.EdgeDistance(
        _c.transform, _c.CombatRadius,
        _c.CurrentTarget, GetTargetRadius());
  }
}