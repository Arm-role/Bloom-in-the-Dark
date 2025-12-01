using UnityEngine;

public class ChaseState : IEnemyState
{
    private EnemyController _c;

    public ChaseState(EnemyController c)
    {
        _c = c;
    }

    public void Enter()
    {
        // nothing necessary
    }

    public void Exit() { }

    public void ManualUpdate()
    {
        var player = _c.Player;
        if (player == null) return;

        float dist = Vector2.Distance(_c.transform.position, player.position);

        // Dead?
        if (_c.Data.IsDead)
        {
            _c.ChangeState(_c.DeadState);
            return;
        }

        // Attack ready?
        if (_c.Combat.AnySkillReadyInRange(dist))
        {
            _c.ChangeState(_c.AttackState);
            return;
        }
    }

    public void ManualFixedUpdate()
    {
        var p = _c.Player;
        if (p == null) return;

        Vector2 dir = p.position - _c.transform.position;

        _c.Data.SetMoveDirection(dir.normalized);

        // Steering-based movement
        _c.Movement.MoveTowards(dir);
    }
}
