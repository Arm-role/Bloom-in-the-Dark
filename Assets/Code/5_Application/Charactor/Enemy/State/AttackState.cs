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
        _c.Locomotion.Stop();
        _brain?.StopPattern();
        _brain?.Tick(_c, _c.Player);
    }

    public void Exit() => _brain?.StopPattern();

    public void ManualUpdate()
    {
        if (_c.Data.IsDead)
        {
            _c.ChangeState(_c.DeadState);
            return;
        }

        // AttackState จะอยู่เฉยๆให้ Pattern ทำงาน
        // ถ้าฉุด target ออกห่างเกิน ก็กลับไป Chase
        float dist = Vector2.Distance(_c.transform.position, _c.Player.position);
        if (dist > _c.Sensor.chaseRadius)
        {
            _c.ChangeState(_c.ChaseState);
        }
    }

    public void ManualFixedUpdate() { }
}