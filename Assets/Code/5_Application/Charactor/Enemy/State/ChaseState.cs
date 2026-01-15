using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly EnemyController _c;
    private float _repathTimer = 0f;
    private const float BASE_INTERVAL = 0.22f;

    public ChaseState(EnemyController c) { _c = c; }

    public void Enter()
    {
        _repathTimer = 0f;
    }

    public void Exit() { }

    public void ManualUpdate()
    {
        if (_c.Player == null) return;

        float d = Vector3.Distance(_c.transform.position, _c.Player.position);
        bool hasLOS = _c.Sensor.HasLOS(_c.Player);

        if (hasLOS && _c.Combat.AnySkillReadyInRange(d))
        {
            _c.ChangeState(_c.AttackState);
            return;
        }


        _repathTimer -= Time.deltaTime;
        if (_repathTimer <= 0f)
        {
            _repathTimer = BASE_INTERVAL;
            // schedule rebuild through hub (Director will dedupe/aggregate)
            FlowFieldRequestHub.Instance.RequestRebuild("AttackPlayer", _c.Player.position);
        }
    }

    public void ManualFixedUpdate()
    {
        // movement handled centrally by EnemyMovement reading FlowFieldManager
    }
}
