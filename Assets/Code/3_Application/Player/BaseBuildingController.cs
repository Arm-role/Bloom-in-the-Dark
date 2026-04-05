using System;
using UnityEngine;

public class BaseBuildingController : MonoBehaviour,
  ICombatEntity,
  IDamageable,
  IHealthable
{
  [SerializeField] private float combatRadius = 0.6f;
  public Transform Transform => transform;
  public float CombatRadius => combatRadius;

  private IStatDatabase _statDatabase;
  private IStatService _statService;

  private BuildingHealth _buildingHealth;
  private BarPresenter<BuildingHealth> _healthPresenter;
  private IBarView BarView { get; set; }

  private IFlashHitView _flashHitView;

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<PlayerHealthResult> OnHeal;

  public bool IsAlive { get; set; } = true;

  private void Awake()
  {
    _flashHitView = GetComponent<IFlashHitView>();
    BarView = GetComponent<IBarView>();

    ComputeCombatRadius();
  }

  public void Initialize(
    IStatDatabase statDatabase,
    IStatService statService,
    BuildingHealth buildingHealth)
  {
    _statDatabase = statDatabase;
    _statService = statService;
    _buildingHealth = buildingHealth;

    _healthPresenter = new BarPresenter<BuildingHealth>(buildingHealth, BarView);
  }

  private void ComputeCombatRadius()
  {
    Collider2D col = GetComponent<Collider2D>();

    if (col == null)
    {
      combatRadius = 0.5f;
      return;
    }

    Bounds b = col.bounds;

    combatRadius =
      Mathf.Max(
          b.extents.x,
          b.extents.y
      );
  }

  // --------------------------
  // Damage
  // --------------------------

  public bool TakeDamage(DamageContext context)
  {
    if (!_buildingHealth.IsAlive)
      return true;

    _flashHitView?.FlashEffect();

    _buildingHealth.TakeDamage(context.Damage);

    bool isDead =
      !_buildingHealth.IsAlive;

    var result =
     new CharacterDamageResult(
         context.Damage,
         transform.position,
         context.HitDirection,
         isDead
     );

    OnDamaged?.Invoke(result);

    if (isDead)
      OnBroken();

    return isDead;
  }

  public void Heal(HealthContext context)
  {
    _buildingHealth.Heal(context.Amount);

    var result = new PlayerHealthResult(
    context.Amount,
    transform.position);

    OnHeal?.Invoke(result);
  }

  // --------------------------
  // Lifecycle
  // --------------------------

  private void OnBroken()
  {
    IsAlive = false;

    //foreach (var col in GetComponents<Collider2D>())
    //  col.enabled = false;

    //gameObject.layer = LayerMask.NameToLayer("DeadBuilding");
  }
}