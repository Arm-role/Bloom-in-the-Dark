using UnityEngine;

public class ChaseState : IEnemyState
{
  private readonly EnemyController _c;

  private float _repathTimer;
  private const float REPTH_INTERVAL = 0.25f;

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

  public void ManualUpdate()
  {
    if (_c.CurrentTarget == null)
    {
      _c.ChangeState(_c.IdleState);
      return;
    }

    float dist =
        Vector2.Distance(
            _c.transform.position,
            _c.CurrentTarget.position
        );

    //bool hasLOS =
    //    _c.Sensor.HasLOS(
    //        _c.CurrentTarget
    //    );

    //// เข้า attack state
    //if (hasLOS &&
    //    _c.Combat.AnySkillReadyInRange(dist))
    //{
    //  _c.ChangeState(
    //      _c.AttackState
    //  );

    //  return;
    //}

    // repath timer
    _repathTimer -= Time.deltaTime;

    if (_repathTimer <= 0f)
    {
      _repathTimer =
          REPTH_INTERVAL;

      _c.NavigationAgent
          .SetTarget(
              _c.CurrentTarget
          );
    }
  }

  public void ManualFixedUpdate()
  {
    // movement handled by steering
  }
}
