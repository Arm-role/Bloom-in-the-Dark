using UnityEngine;

public class AttackState : IEnemyState
{
  private EnemyController _c;
  private EnemyPatternBrain _brain;

  public AttackState(EnemyController c)
  {
    _c = c;
    _brain = c.GetComponent<EnemyPatternBrain>();
  }

  public void Enter()
  {
    //_c.Locomotion.StopMovement();
    Debug.Log($"[AttackState] Enter — brain={_brain}, target={_c.CurrentTarget}");

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

  private float EdgeDist()
  {
    if (_c.CurrentTarget == null) return float.MaxValue;
    float targetRadius = _c.CurrentTarget.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    return CombatDistanceUtility.EdgeDistance(
        _c.transform, _c.CombatRadius,
        _c.CurrentTarget, targetRadius);
  }
}