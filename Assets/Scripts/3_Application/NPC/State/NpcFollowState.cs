using UnityEngine;

public class NpcFollowState : INpcState
{
    private readonly NpcController _c;

    private float _repathTimer;
    private const float REPATH_INTERVAL = 0.5f;

    public NpcFollowState(NpcController c) { _c = c; }

    public void Enter()
    {
        _repathTimer = 0f;
    }

    public void Exit() { }

    public void Tick()
    {
        if (_c.CurrentTarget == null) { _c.ChangeState(_c.IdleState); return; }

        if (_c.NavigationAgent.HasReachedTarget()) return;

        _repathTimer -= Time.deltaTime;
        if (_repathTimer <= 0f)
        {
            _repathTimer = REPATH_INTERVAL;
            _c.NavigationAgent.RequestFlowUpdateImmediate();
        }
    }

    public void FixedTick()
    {
        if (!_c.NavigationAgent.HasValidFlow || _c.NavigationAgent.HasReachedTarget())
        {
            _c.Locomotion.Stop();
            return;
        }

        _c.Locomotion.ApplySteering(_c.Steering.Sample());
    }
}
