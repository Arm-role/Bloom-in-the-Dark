using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyController _controller;

    public IdleState(EnemyController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Locomotion.Stop();
        _controller.State.SetMoveDirection(Vector2.zero);
    }

    public void Exit() { }

    public void ManualUpdate()
    {
        var sensor = _controller.Sensor;
        var player = _controller.Player;

        if (player == null) return;

        // detect player
        if (sensor.CheckDetect(player))
        {
            _controller.ChangeState(_controller.ChaseState);
        }
    }

    public void ManualFixedUpdate()
    {
    }
}
