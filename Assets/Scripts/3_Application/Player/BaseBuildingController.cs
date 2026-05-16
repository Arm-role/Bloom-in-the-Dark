using System;
using UnityEngine;

public class BaseBuildingController : MonoBehaviour,
    ICombatEntity, IDamageable, IHealthable
{
  [SerializeField] private BuildingConfig config;
  [SerializeField] private float combatRadius = 0.6f;

  public Transform Transform => transform;
  public float CombatRadius => combatRadius;
  public bool IsAlive { get; private set; } = true;

  private BuildingHealth _buildingHealth;
  private BarPresenter<BuildingHealth> _healthPresenter;
  private IBarView _barView;
  private IFlashHitView _flashHitView;

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<PlayerHealthResult> OnHeal;
  public event Action<GameObject> RemoveObject;

  private void Awake()
  {
    _flashHitView = GetComponent<IFlashHitView>();
    _barView = GetComponent<IBarView>();
    ComputeCombatRadius();

    if (config != null)
      SetupHealth(config.MaxHP);
  }
  public void Initialize(Action<GameObject> removeObject)
  {
    RemoveObject = removeObject;

    if (_buildingHealth != null)
    {
      _buildingHealth.Fill();
      IsAlive = true;
    }
  }

  public void SetupHealth(float maxHP)
  {
    _buildingHealth = new BuildingHealth(maxHP);
    _healthPresenter = new BarPresenter<BuildingHealth>(_buildingHealth, _barView);
    IsAlive = true;
  }

  public void Initialize(
      IStatDatabase statDatabase,
      IStatService statService,
      BuildingHealth buildingHealth)
  {
    _buildingHealth = buildingHealth;
    _healthPresenter = new BarPresenter<BuildingHealth>(buildingHealth, _barView);
    IsAlive = true;
  }

  public bool TakeDamage(DamageContext context)
  {
    if (_buildingHealth == null || !_buildingHealth.IsAlive)
      return true;

    _flashHitView?.FlashEffect();
    _buildingHealth.TakeDamage(context.Damage);

    bool isDead = !_buildingHealth.IsAlive;

    OnDamaged?.Invoke(new CharacterDamageResult(
        context.Damage,
        transform.position,
        context.HitDirection,
        isDead));

    if (isDead) OnBroken();

    return isDead;
  }

  public void Heal(HealthContext context)
  {
    _buildingHealth?.Heal(context.Amount);

    OnHeal?.Invoke(new PlayerHealthResult(
        context.Amount,
        transform.position));
  }

  private void OnBroken()
  {
    IsAlive = false;
    Debug.Log("Broken!");
    EnemyManager.Instance?.NotifyTargetDestroyed(transform);
    RemoveObject?.Invoke(gameObject);
  }

  private void ComputeCombatRadius()
  {
    var col = GetComponent<Collider2D>();
    if (col == null) { combatRadius = 0.5f; return; }
    var b = col.bounds;
    combatRadius = Mathf.Max(b.extents.x, b.extents.y);
  }
}