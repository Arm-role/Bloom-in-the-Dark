public class NpcIdleState : INpcState
{
    private readonly NpcController _c;

    public NpcIdleState(NpcController c) { _c = c; }

    public void Enter() { }
    public void Exit() { }

    public void Tick()
    {
        if (_c.CurrentTarget != null)
            _c.ChangeState(_c.FollowState);
    }

    public void FixedTick()
    {
        _c.Locomotion.Stop();
    }
}
