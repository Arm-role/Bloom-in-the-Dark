using UnityEngine;

public class DummyController : EntityController
{
  public EnemyHealth Health;

  // ======================================================
  // POOL — full setup here
  // ======================================================

  public void Initialize(int hp = 10)
  {
    Health = new EnemyHealth(hp);
    _healthPresenter = new BarPresenter<EnemyHealth>(Health, HealthBarView);
  }

  public override void OnSpawnFromPool(GameObject ob)
  {
    FlashHitView?.SetObject();
    //_current = null;
  }

  public override void OnReturnToPool(GameObject ob) { }
  public override bool TakeDamage(DamageContext context)
  {
    FlashHitView?.FlashEffect();
    Health.TakeDamage(context.Damage);

    Knockback.ApplyKnockback(context.HitDirection, context.KnockForce, context.KnockDration);

    if (!Health.IsAlive)
      OnDied();

    return !Health.IsAlive;
  }

  private void OnDied()
  {
    RequestDestruction();
  }
}