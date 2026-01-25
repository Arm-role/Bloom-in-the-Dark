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
        var sensor = _c.Sensor;
        var player = _c.Player;

        if (player == null) return;

        // detect player
        if (sensor.CheckDetect(player))
        {
            _c.ChangeState(_c.ChaseState);
        }
    }

    public void ManualFixedUpdate()
    {
    }
}
