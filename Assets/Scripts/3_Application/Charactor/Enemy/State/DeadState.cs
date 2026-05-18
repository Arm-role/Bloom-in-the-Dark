using UnityEngine;

public class DeadState : IEnemyState
{
  private EnemyController _c;

  public DeadState(EnemyController c)
  {
    _c = c;
  }

  public void Enter()
  {
    _c.Locomotion.StopMovement();
    _c.OnRequestDisableCollision();
    _c.AnimationSystem.RaiseFinished += StartDestroy;
  }

  public void Exit()
  {
    _c.AnimationSystem.RaiseFinished -= StartDestroy;
  }

  public void ManualUpdate() { }
  public void ManualFixedUpdate() { }

  private void StartDestroy()
  {
    _c.AnimationSystem.HideVisual();
    _c.AnimationSystem.Reset();
    _c.StartCoroutine(DestroyAfter());
  }
  private System.Collections.IEnumerator DestroyAfter()
  {
    yield return new WaitForSeconds(1.5f);
    _c.RequestDestruction();
  }
}
