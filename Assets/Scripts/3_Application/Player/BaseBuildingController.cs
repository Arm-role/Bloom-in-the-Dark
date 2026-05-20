using System;
using UnityEngine;

public class BaseBuildingController : MonoBehaviour,
    ICombatEntity, IDamageable, IHealthable, IBuildingController
{
    [SerializeField] private BuildingConfig config;
    [SerializeField] private float combatRadius = 0.6f;

    public Transform Transform => transform;
    public float CombatRadius => combatRadius;
    public bool IsAlive => _healthController?.IsAlive ?? false;

    private BuildingHealthController _healthController;
    private BarPresenter<BuildingHealth> _healthPresenter;
    private IBarView _barView;
    private IFlashHitView _flashHitView;

    public event Action<CharacterDamageResult> OnDamaged;
    public event Action<PlayerHealthResult> OnHeal;
    public event Action<GameObject> RemoveObject;

    // fired ตอน building HP = 0 — GameOver / defeat handlers subscribe ที่นี่
    public event Action OnBuildingDestroyed;

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
        _healthController?.Fill();
    }

    public void SetupHealth(float maxHP)
    {
        _healthPresenter?.Dispose();
        _healthController = new BuildingHealthController(maxHP);
        _healthPresenter = new BarPresenter<BuildingHealth>(_healthController.Health, _barView);
    }

    private void OnDestroy()
    {
        _healthPresenter?.Dispose();
    }

    public bool TakeDamage(DamageContext context)
    {
        // ไม่มี health controller = setup ไม่ครบ → ไม่ตาย (ดีกว่า instant-death)
        if (_healthController == null)
        {
            Debug.LogWarning($"[BaseBuildingController] {name} has no health controller — assign config หรือเรียก Initialize");
            return false;
        }

        _flashHitView?.FlashEffect();
        bool isDead = _healthController.TakeDamage(context.Damage);

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
        _healthController?.Heal(context.Amount);
        OnHeal?.Invoke(new PlayerHealthResult(context.Amount, transform.position));
    }

    protected virtual void OnBroken()
    {
        EnemyManager.Instance?.NotifyTargetDestroyed(transform);
        RemoveObject?.Invoke(gameObject);
        OnBuildingDestroyed?.Invoke();
    }

    private void ComputeCombatRadius()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) { combatRadius = 0.5f; return; }
        var b = col.bounds;
        combatRadius = Mathf.Max(b.extents.x, b.extents.y);
    }
}
