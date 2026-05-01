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

    // ✅ delegate ให้ service จัดการ — service มี threshold guard อยู่แล้ว
    var flowTarget = _c.CurrentTarget.GetComponent<FlowFieldTarget>();
    if (flowTarget != null)
    {
      FlowFieldNavigationService.Instance.EnsureField(
          flowTarget.FlowKey,
          _c.FlowFieldOwner.Footprint,
          _c.CurrentTarget.position);
    }
  }

  public void ManualFixedUpdate()
  {
    // movement handled by steering
  }
}
