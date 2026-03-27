using System;
using UnityEngine;

[RequireComponent(
    typeof(EnemySteering),
    typeof(EnemyLocomotion),
    typeof(EnemySensor))]
public class EnemyController : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
  [SerializeField] private Transform AnimationViewRoot;
  [SerializeField] private CharacterAnimationLibrary animationLibrary;
  
  [Header("Cofig")]
  public EnemyConfig config;
  public EnemyType Type;

  [Header("References")]
  public Transform Player;


  public EnemyLocomotion Locomotion { get; private set; }
  public EnemySteering Steering { get; private set; }
  public EnemySensor Sensor { get; private set; }
  public EnemyCombat Combat { get; private set; }
  public EnemyState State { get; private set; }
  public HealthResource Health { get; private set; }
  public EnemyTargetSelector EnemyTargetSelector { get; private set; }
  public CharacterAnimationSystem AnimationSystem { get; private set; }

  public IBarView HealthBarView { get; private set; }
  public IFlashHitView FlashHitView { get; private set; }

  public IEnemyState IdleState { get; private set; }
  public IEnemyState ChaseState { get; private set; }
  public IEnemyState AttackState { get; private set; }
  public IEnemyState DeadState { get; private set; }

  private BarPresenter<HealthResource> _healthPresenter;
  private ILootableHandler _lootableHandler;
  private Collider2D[] _collider2Ds;

  private IEnemyState _current;

  private bool _isMovementStopped = false;
  private float _stopUntilTime = 0f;

  private bool _holdPosition = false;

  private int _sensorTickId = -1;
  private int _stateTickId = -1;
  public bool IsAlive { get; set; }

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<GameObject> OnRequestDestruction;
  public event Action<WorldAction> OnGetLootable;

  // ======================================================
  // AWAKE — prepare references only
  // ======================================================
  private void Awake()
  {
    Locomotion = GetComponent<EnemyLocomotion>();
    Steering = GetComponent<EnemySteering>();
    Sensor = GetComponent<EnemySensor>();

    Combat = gameObject.AddComponent<EnemyCombat>();
    State = new EnemyState(transform);

    FlashHitView = GetComponent<IFlashHitView>();
    HealthBarView = GetComponent<IBarView>();

    _lootableHandler = GetComponent<ILootableHandler>();
    _collider2Ds = GetComponents<Collider2D>();

    IdleState = new IdleState(this);
    ChaseState = new ChaseState(this);
    AttackState = new AttackState(this);
    DeadState = new DeadState(this);

  }

  // ======================================================
  // POOL — full setup here
  // ======================================================

  public void Initialize()
  {
    var factory = new CharacterAnimationFactory();
    var characterAnimation = AnimationViewRoot.GetComponent<ICharacterAnimationView>();

    AnimationSystem = factory.Create(
      animationLibrary,
      characterAnimation
      );

    OnDamaged += AnimationSystem.HandleDamage;
  }

  public void Setup(Transform player, float moveSpeed = 3f, int hp = 10)
  {
    Player = player;

    Steering.moveSpeed = moveSpeed;
    State.MoveSpeed = moveSpeed;

    Health = new HealthResource(hp);
    _healthPresenter = new BarPresenter<HealthResource>(Health, HealthBarView);

    Combat.Initialize(player);
    OnRequestEnableCollision();
  }

  public void OnSpawnFromPool(GameObject ob)
  {
    FlashHitView?.SetObject();

    ApplyConfig();

    Locomotion.StopMovement();

    Debug.Log(gameObject.activeSelf);
    AnimationSystem?.ShowVisual();

    _current = null;
    ChangeState(IdleState);

    if (AITickManager.Instance != null)
    {
      _sensorTickId = AITickManager.Instance.Register(TickSensor, 8f);
      _stateTickId = AITickManager.Instance.Register(TickState, 15f);
    }

    Combat.OnRequestStopMovement += OnRequestStopMovement;
    Combat.OnRequestDash += OnRequestDash;
    Combat.OnRequestHoldPosition += OnRequestHoldPosition;

    Combat.OnPlayAttack += HandleAttackAnimation;
    Combat.OnPlayPrepareDash += HandlePrepareDashAnimation;
    Combat.OnPlayDash += HandleDashAnimation;
    Combat.OnPlayEndDash += HandleEndDashAnimation;

    Locomotion.OnVelocityChanged += HandleMovementDirectionAnimation;

    EnemyManager.Instance?.RegisterEnemy(this);
  }

  public void OnReturnToPool(GameObject ob)
  {
    if (AITickManager.Instance != null)
    {
      if (_sensorTickId >= 0) AITickManager.Instance.Unregister(_sensorTickId);
      if (_stateTickId >= 0) AITickManager.Instance.Unregister(_stateTickId);
    }

    Combat.OnRequestStopMovement -= OnRequestStopMovement;
    Combat.OnRequestDash -= OnRequestDash;
    Combat.OnRequestHoldPosition -= OnRequestHoldPosition;

    Combat.OnPlayAttack -= HandleAttackAnimation;
    Combat.OnPlayPrepareDash -= HandlePrepareDashAnimation;
    Combat.OnPlayDash -= HandleDashAnimation;
    Combat.OnPlayEndDash -= HandleEndDashAnimation;

    Locomotion.OnVelocityChanged -= HandleMovementDirectionAnimation;

    OnDamaged -= AnimationSystem.HandleDamage;

    EnemyManager.Instance?.UnregisterEnemy(this);
  }

  private void ApplyConfig()
  {
    Locomotion.BaseSpeed = config.BaseSpeed;
    Locomotion.Accel = config.Accel;
    Locomotion.TurnSharpness = config.TurnSharpness;
    Locomotion.KnockbackFriction = config.KnockbackFriction;

    Steering.moveSpeed = config.moveSpeed;
    Steering.accel = config.Accel;
    Steering.turnSpeed = config.turnSpeed;

    Steering.flowKey = "AttackPlayer";

    Steering.obstacleDist = config.obstacleDist;
    Steering.obstacleStrength = config.obstacleStrength;
    Steering.avoidAngle = config.avoidAngle;
    Steering.obstacleMask = config.obstacleMask;

    Steering.separationRadius = config.separationRadius;
    Steering.separationStrength = config.separationStrength;
    Steering.enemyLayerMask = config.enemyLayer;

    Steering.cornerRadius = config.cornerRadius;
    Steering.cornerPush = config.cornerPush;
    Steering.passageProbeDist = config.passageProbeDist;
    Steering.narrowThreshold = config.narrowThreshold;
    Steering.centerStrength = config.centerStrength;
    Steering.narrowSpeedMul = config.narrowSpeedMul;

    Sensor.targetMask = config.targetMask;
    Sensor.obstacleMask = config.obstacleMask;
  }


  private void Update()
  {
    if (_isMovementStopped && Time.time >= _stopUntilTime)
      _isMovementStopped = false;
  }

  private void FixedUpdate()
  {
    if (!Health.IsAlive) return;

    _current?.ManualFixedUpdate();

    if (_isMovementStopped || _holdPosition)
    {
      Locomotion.StopMovement();
      return;
    }

    SteeringResult result = Steering.TickSteering();
    Locomotion.ApplySteering(result);
  }

  public void TakeDamage(DamageContext context)
  {
    if (!Health.IsAlive)
      return;

    FlashHitView?.FlashEffect();

    Health.TakeDamage(context.Damage);
    Locomotion.ApplyKnockback(context.HitDirection, context.KnockForce, context.KnockDration);

    bool isDead = !Health.IsAlive;

    var result = new CharacterDamageResult(
      context.Damage,
      context.HitDirection,
      isDead);

    OnDamaged?.Invoke(result);

    if (isDead)
    {
      GiveReward(context);
      ChangeState(DeadState);
    }
  }
  public void GiveReward(DamageContext context)
  {
    var result = new WorldAction();

    (int exp, ItemStack[] loot) lootAll;

    var item = context.Intent.SourceItem.Data;

    if (item.HasTag(TagLibrary.Get("Tool")))
      lootAll = _lootableHandler.GetHarvestLoot(item);
    else
      lootAll = _lootableHandler.GetHarvestLoot();

    result.Exp = lootAll.exp;

    foreach (var stack in lootAll.loot)
      result.ItemRewards.Add(stack);

    OnGetLootable?.Invoke(result);
  }
  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  // =============================
  // SENSOR SYSTEM
  // =============================
  private void TickSensor()
  {
    if (Player == null) return;

    Sensor.CheckDetect(Player);

    if (_current == IdleState && Sensor.DetectedTarget != null)
      ChangeState(ChaseState);
  }

  private void TickState()
  {
    _current?.ManualUpdate();
  }

  public void ChangeState(IEnemyState newState)
  {
    if (_current == newState) return;
    _current?.Exit();
    _current = newState;
    Debug.Log(newState.ToString());
    _current?.Enter();
  }

  public void AddSkill(IEnemySkill s)
  {
    Combat.AddSkill(s);
    Sensor.AutoSetup(Combat);
  }

  // =============================
  // STOP & DASH
  // =============================
  private void OnRequestStopMovement(float duration)
  {
    _isMovementStopped = true;
    _stopUntilTime = Time.time + duration;
  }

  private void OnRequestHoldPosition(bool hold)
  {
    _holdPosition = hold;
  }

  private void OnRequestDash(Vector2 impulse, float duration)
  {
    Locomotion.ApplyDash(impulse, duration);
    Combat.OnPlayDash?.Invoke();
  }

  public void OnRequestEnableCollision()
  {
    foreach (var collider2D in _collider2Ds)
      collider2D.enabled = true;
  }

  public void OnRequestDisableCollision()
  {
    foreach (var collider2D in _collider2Ds)
      collider2D.enabled = false;
  }

  // =============================
  // Animation
  // =============================

  private void HandleMovementDirectionAnimation(Vector2 dir)
  {
    AnimationSystem.SetMoveDirection(dir);
    AnimationSystem.SetLookDirection(dir);
  }

  private void HandleAttackAnimation(string attackTag)
  {
    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.AttackTag;

    if (tag == null) return;

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    AnimationSystem.Handle(command);
  }

  private void HandlePrepareDashAnimation()
  {
    Debug.Log("PrepareDash");

    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.PrepareDashTag;

    if (tag == null) return;

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    AnimationSystem.Handle(command);
  }

  private void HandleDashAnimation()
  {
    Debug.Log("Dash");

    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.DashTag;

    if (tag == null) return;

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    AnimationSystem.Handle(command);
  }

  private void HandleEndDashAnimation()
  {
    Debug.Log("EndDash");

    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.EndDashTag;

    if (tag == null) return;

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    AnimationSystem.Handle(command);
  }




  private void OnDrawGizmosSelected()
  {
    if (Combat == null)
      return;

    var skills = Combat.GetSkills();

    foreach (var skill in skills)
    {
      if (skill is DashSkill dash)
      {
        dash.DrawGizmos();
      }
    }
  }
}
