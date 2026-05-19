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
    // หยุด skill coroutine ที่อาจกำลังรันอยู่ (เช่น AOESlam mid-bezier)
    // ป้องกัน skill ยิง animation event ทับ death animation
    _c.Combat?.CancelAllSkills();

    _c.Locomotion.StopMovement();
    _c.OnRequestDisableCollision();

    // lock animation หลังจาก death animation เริ่มเล่นไปแล้ว (RaiseDamaged → HandleDamage)
    // ป้องกัน skill animation ที่หลุดมาทับ death
    _c.AnimationSystem.LockAnimation();

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
