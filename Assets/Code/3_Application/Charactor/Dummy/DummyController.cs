using System;
using UnityEngine;

public class DummyController : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
  public HealthResource Health { get; private set; }
  public KnockbackSimulator Knockback { get; private set; }

  public ICharacterAnimationView AnimView { get; private set; }
  public IBarView HealthBarView { get; private set; }
  public IFlashHitView FlashHitView { get; private set; }

  private BarPresenter<HealthResource> _healthPresenter;

  private IEnemyState _current;

  public bool IsAlive { get; set; }

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<GameObject> OnRequestDestruction;

  // ======================================================
  // AWAKE — prepare references only
  // ======================================================
  private void Awake()
  {
    Knockback = GetComponent<KnockbackSimulator>();

    AnimView = GetComponent<ICharacterAnimationView>();
    FlashHitView = GetComponent<IFlashHitView>();
    HealthBarView = GetComponent<IBarView>();
  }

  // ======================================================
  // POOL — full setup here
  // ======================================================

  public void Initialize(int hp = 10)
  {
    Health = new HealthResource(hp);
    _healthPresenter = new BarPresenter<HealthResource>(Health, HealthBarView);
  }

  public void OnSpawnFromPool(GameObject ob)
  {
    FlashHitView?.SetObject();
    _current = null;
  }

  public void OnReturnToPool(GameObject ob)
  {
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  public void TakeDamage(DamageContext context)
  {
    FlashHitView?.FlashEffect();
    Health.TakeDamage(context.Damage);

    Knockback.ApplyKnockback(context.HitDirection, context.KnockForce, context.KnockDration);

    if (!Health.IsAlive)
      OnDied();
  }

  private void OnDied()
  {
    RequestDestruction();
  }
}