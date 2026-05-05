using System;
using UnityEngine;

public abstract class EntityController : MonoBehaviour,
 ICombatEntity, IDamageable, IDestructible, IPoolable<GameObject>
{
  [SerializeField] private float combatRadius = 0.6f;
  public Transform Transform => transform;
  public float CombatRadius => combatRadius;

  public KnockbackSimulator Knockback { get; protected set; }

  public ICharacterAnimationView AnimView { get; protected set; }
  public IBarView HealthBarView { get; protected set; }
  public IFlashHitView FlashHitView { get; protected set; }

  protected BarPresenter<EnemyHealth> _healthPresenter;

  public bool IsAlive { get; set; }

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<GameObject> OnRequestDestruction;

  // ======================================================
  // POOL — full setup here
  // ======================================================

  private void Awake()
  {
    BuildComponent();
  }

  protected virtual void BuildComponent()
  {
    Knockback = GetComponent<KnockbackSimulator>();

    AnimView = GetComponent<ICharacterAnimationView>();
    FlashHitView = GetComponent<IFlashHitView>();
    HealthBarView = GetComponent<IBarView>();
  }

  public abstract void OnSpawnFromPool(GameObject ob);
  public abstract void OnReturnToPool(GameObject ob);
  public abstract bool TakeDamage(DamageContext context);

  protected void RaiseDamaged(CharacterDamageResult result)
  {
    OnDamaged?.Invoke(result);
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }
}