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
    _c.Locomotion.StopMovement();

    _brain?.StopPattern();

    var target = _c.CurrentTarget;

    if (target != null)
    {
      _brain?.Tick(_c, target);
    }
  }

  public void Exit()
  {
    _brain?.StopPattern();
  }

  public void ManualUpdate()
  {
    if (!_c.Health.IsAlive)
    {
      _c.ChangeState(_c.DeadState);
      return;
    }

    var target = _c.CurrentTarget;

    if (target == null)
    {
      _c.ChangeState(_c.ChaseState);
      return;
    }

    float dist =
        Vector2.Distance(
            _c.transform.position,
            target.position
        );

    if (!_c.Combat.AnySkillReadyInRange(dist))
    {
      _c.ChangeState(_c.ChaseState);
    }
  }

  public void ManualFixedUpdate() { }
}