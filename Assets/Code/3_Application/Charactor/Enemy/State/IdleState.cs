using UnityEngine;

public class IdleState : IEnemyState
{
  private EnemyController _c;

  public IdleState(EnemyController c)
  {
    _c = c;
  }

  public void Enter()
  {
    _c.Locomotion.StopMovement();
    _c.State.SetMoveDirection(Vector2.zero);
  }

  public void Exit() { }

  public void ManualUpdate()
  {
    if (_c.CurrentTarget != null)
    {
      _c.ChangeState(_c.ChaseState);
    }
  }

  public void ManualFixedUpdate() { }
}
