using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour,
  IGameStateListener,
  ICombatEntity,
  IDamageable,
  IEnergyable,
  IHealthable,
  IDestructible,
  IPoolable<GameObject>
{

  [SerializeField] private float combatRadius = 0.6f;
  public Transform Transform => transform;
  public float CombatRadius => combatRadius;

  [SerializeField] private Transform AnimationViewRoot;

  // delay หลัง death animation จบ ก่อน SetActive(false) — เลียนแบบ Enemy.DeadState
  [SerializeField] private float _deathHideDelay = 0.5f;

  private Rigidbody2D _rb;

  private IPlayerInput _playerInput;
  private IStatDatabase _statDatabase;

  private IMovement _playerMovement;
  private PlayerState _playerState;
  private IStatService _statService;

  private KnockbackSimulator _knockback;

  private BarPresenter<PlayerHealth> _healthPresenter;
  private BarPresenter<PlayerEnergy> _energyPresenter;

  private CharacterAnimationSystem _playerAnimationSystem;
  private PlayerHealth _playerHealth;

  public PlayerInteractor Interactor { get; private set; }

  private IBarView BarView { get; set; }
  private IBarView EnergyBarView { get; set; }

  private IFlashHitView _flashHitView;

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<PlayerEnergyResult> OnEnergy;

  public event Action<PlayerHealthResult> OnHeal;

  public event Action<GameObject> OnRequestDestruction;

  // fired แทน RequestDestruction ตอน player ตาย — respawn system รับช่วงต่อ
  public event Action OnPlayerDied;

  public bool IsAlive { get; set; } = true;
  private bool isGamePlayStat;
  private bool _initialized;

  private void Awake()
  {
    _flashHitView = GetComponent<IFlashHitView>();

    foreach (var bar in GetComponents<IBarView>())
    {
      if (bar.Name == "HP")
      {
        BarView = bar;
      }
      else if (bar.Name == "Energy")
      {
        EnergyBarView = bar;
      }
    }

    _rb = GetComponent<Rigidbody2D>();
  }

  public void Initialize(
    IPlayerInput playerInput,
    IStatDatabase statDatabase,
    PlayerState playerState,
    IStatService statService,
    PlayerHealth playerHealth,
    PlayerEnergy playerEnergy,
    PlayerInteractor interactor,
    CharacterAnimationSystem animationSystem)
  {
    _playerInput = playerInput;
    _statDatabase = statDatabase;
    _playerState = playerState;
    _statService = statService;

    _knockback = GetComponent<KnockbackSimulator>();

    Interactor = interactor;

    _playerAnimationSystem = animationSystem;
    _playerAnimationSystem.Initializa(AnimationViewRoot.GetComponent<ICharacterAnimationView>());

    _playerHealth = playerHealth;
    _healthPresenter = new BarPresenter<PlayerHealth>(playerHealth, BarView);
    _energyPresenter = new BarPresenter<PlayerEnergy>(playerEnergy, EnergyBarView);

    _playerMovement = new PlayerMovement(_statDatabase.MoveSpeed, _statService);

    _initialized = true;
    SubscribeAnimation();
  }

  private void OnEnable()
  {
    if (_initialized)
      SubscribeAnimation();
  }

  private void OnDisable()
  {
    if (_initialized)
      UnsubscribeAnimation();
  }

  // re-subscribe ตอน SetActive(true) หลัง respawn — OnDisable ตัด subscription ทิ้งตอนตาย
  private void SubscribeAnimation()
  {
    UnsubscribeAnimation();
    _playerState.OnMoveDirection += _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection += _playerAnimationSystem.SetLookDirection;
    OnDamaged += _playerAnimationSystem.HandleDamage;
  }

  private void UnsubscribeAnimation()
  {
    _playerState.OnMoveDirection -= _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection -= _playerAnimationSystem.SetLookDirection;
    OnDamaged -= _playerAnimationSystem.HandleDamage;
  }

  public void OnGameStateChanged(EGameState state)
  {
    isGamePlayStat = state == EGameState.Gameplay || state == EGameState.Inventory;
  }

  public void Update()
  {
    if (!isGamePlayStat)
      return;

    // ตายแล้ว — หยุดป้อน input ไม่ให้ Animator flip เข้า move blend tree ทับ death clip
    if (_playerHealth != null && !_playerHealth.IsAlive)
      return;

    if (Interactor != null && Interactor.IsBusy())
    {
      _playerState.UpdateMoveDirection(Vector2.zero);
      return;
    }

    _playerState.UpdateMoveDirection(_playerInput.MoveDirection);
  }

  public void FixedUpdate()
  {
    if (!isGamePlayStat) return;

    // ตายแล้ว — หยุด velocity ค้าง ไม่ให้ player ไถลตอนเล่น death animation
    if (_playerHealth != null && !_playerHealth.IsAlive)
    {
      _rb.velocity = Vector2.zero;
      return;
    }

    if (Interactor != null && Interactor.IsBusy())
    {
      _rb.velocity = Vector2.zero;
      return;
    }

    if (_knockback != null && _knockback.IsKnockbacking)
      return;

    Vector2 direction = _playerInput.MoveDirection;
    Vector2 velocity = _playerMovement.CalculateVelocity(direction);
    _rb.velocity = velocity;
  }

  // --------------------------
  // Energy
  // --------------------------

  public void AddEnergy(EnergyContext context)
  {
    Interactor.TryExecute(
      new IncreaseEnergyCommand(context.Energy));

    var result = new PlayerEnergyResult(
     context.Energy,
     transform.position);

    OnEnergy?.Invoke(result);
  }

  public void EnergyFill()
  {
    Interactor.EnergyFill();
  }

  // --------------------------
  // Damage
  // --------------------------

  public bool TakeDamage(DamageContext context)
  {
    // กันตีศพระหว่าง respawn countdown — HP=0 ถือว่าตายแล้ว ไม่รับ damage จนกว่าจะ respawn
    if (_playerHealth != null && !_playerHealth.IsAlive)
      return true;

    _flashHitView?.FlashEffect();

    Interactor.TryExecute(new TakeDamageCommand(context.Damage));

    bool isDead = _playerHealth != null && !_playerHealth.IsAlive;

    if (context.KnockForce != 0 && context.KnockDration != 0)
      _knockback?.ApplyKnockback(
        context.HitDirection,
        context.KnockForce,
        context.KnockDration);

    var result = new CharacterDamageResult(
     context.Damage,
     transform.position,
     context.HitDirection,
     isDead);

    OnDamaged?.Invoke(result);

    if (isDead) OnDied();

    return isDead;
  }

  public void Heal(HealthContext context)
  {
    Interactor.HealthHeal(context.Amount);

    var result = new PlayerHealthResult(
    context.Amount,
    transform.position);

    OnHeal?.Invoke(result);
  }

  // --------------------------
  // Lifecycle
  // --------------------------

  public void OnSpawnFromPool(GameObject ob) { }
  public void OnReturnToPool(GameObject ob) { }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  private void OnDied()
  {
    // ไม่ RequestDestruction อีกแล้ว — respawn system จะ handle
    OnPlayerDied?.Invoke();

    // หยุด movement + lock animation เหมือน Enemy.DeadState
    // กัน clip อื่น (walk/idle/hit) เล่นทับ death clip จน Animation_Finished ยิงผิดจังหวะ
    _playerState.UpdateMoveDirection(Vector2.zero);
    _rb.velocity = Vector2.zero;
    _playerAnimationSystem.LockAnimation();

    // รอ death animation เล่นจบ (event Animation_Finished บน clip) แล้วค่อยซ่อนตัว
    _playerAnimationSystem.RaiseFinished += HandleDeathAnimationFinished;
  }

  private void HandleDeathAnimationFinished()
  {
    _playerAnimationSystem.RaiseFinished -= HandleDeathAnimationFinished;

    // เลียนแบบ Enemy.DeadState: anim จบ → ปิด renderer ก่อน → delay สั้น ๆ → ค่อยปิด GameObject
    _playerAnimationSystem.HideVisual();
    StartCoroutine(DeactivateAfterDeath());
  }

  private System.Collections.IEnumerator DeactivateAfterDeath()
  {
    yield return new WaitForSeconds(_deathHideDelay);
    gameObject.SetActive(false);
  }

  // เรียกโดย PlayerRespawnController หลังหมด timer
  public void Respawn(Vector3 position, float hpPercent)
  {
    gameObject.SetActive(true);
    transform.position = position;
    // HandleDeathAnimationFinished ปิด renderer ไว้ — ต้องเปิดกลับก่อน Reset
    _playerAnimationSystem?.ShowVisual();
    _playerAnimationSystem?.Reset();

    if (_playerHealth == null) return;

    _playerHealth.Fill();
    float damageAmount = _playerHealth.Max * (1f - Mathf.Clamp01(hpPercent));
    if (damageAmount > 0f)
      _playerHealth.TakeDamage(damageAmount);
  }
}
