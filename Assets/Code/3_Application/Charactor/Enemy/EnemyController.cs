using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(EnemySteering),
    typeof(EnemyLocomotion),
    typeof(EnemySensor))]
public class EnemyController : EntityController
{
  [SerializeField] private Transform AnimationViewRoot;
  [SerializeField] private CharacterAnimationLibrary animationLibrary;

  [Header("Cofig")]
  [SerializeField] private EnemyConfig config;
  public EnemyType Type;

  [SerializeField]
  private TargetSelectorProfileSO targetSelectorProfile;

  public Transform DefaultTarget { get; private set; }
  public Transform CurrentTarget { get; private set; }

  #region Public Valiable
  public FlowFieldOwner FlowFieldOwner { get; private set; }
  public EnemyLocomotion Locomotion { get; private set; }
  public EnemySteering Steering { get; private set; }
  public EnemySensor Sensor { get; private set; }
  public EnemyCombat Combat { get; private set; }
  public EnemyNavigationAgent NavigationAgent { get; private set; }
  public EnemyState State { get; private set; }
  public EnemyHealth Health { get; private set; }
  public EnemyTargetSelector EnemyTargetSelector { get; private set; }
  public CharacterAnimationSystem AnimationSystem { get; private set; }

  public IEnemyState IdleState { get; private set; }
  public IEnemyState ChaseState { get; private set; }
  public IEnemyState AttackState { get; private set; }
  public IEnemyState DeadState { get; private set; }

  #endregion

  private IEnemyState _current = null;

  private ILootableHandler _lootableHandler;
  private Collider2D[] _collider2Ds;

  private bool _isMovementStopped = false;
  private float _stopUntilTime = 0f;

  private bool _holdPosition = false;
  private bool _navigationPaused;

  private int _sensorTickId = -1;
  private int _stateTickId = -1;

  public event Action<WorldAction> OnGetLootable;

  // ======================================================
  // AWAKE — prepare references only
  // ======================================================

  protected override void BuildComponent()
  {
    base.BuildComponent();

    FlowFieldOwner = GetComponent<FlowFieldOwner>();
    Locomotion = GetComponent<EnemyLocomotion>();
    Steering = GetComponent<EnemySteering>();
    Sensor = GetComponent<EnemySensor>();

    Combat = gameObject.AddComponent<EnemyCombat>();
    State = new EnemyState(transform);

    EnemyTargetSelector = new EnemyTargetSelector(this, targetSelectorProfile);

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
  #region Setup
  public void Initialize()
  {
    var factory = new CharacterAnimationFactory();
    var characterAnimation = AnimationViewRoot.GetComponent<ICharacterAnimationView>();
    NavigationAgent = new EnemyNavigationAgent(this);

    AnimationSystem = factory.Create(
      animationLibrary,
      characterAnimation
      );

    OnDamaged += AnimationSystem.HandleDamage;
  }

  public void Setup(
    float moveSpeed = 3f,
    int hp = 10)
  {
    Steering.moveSpeed = moveSpeed;
    State.MoveSpeed = moveSpeed;

    Health = new EnemyHealth(hp);

    _healthPresenter = new BarPresenter<EnemyHealth>(Health, HealthBarView);

    Combat.Initialize(this);

    OnRequestEnableCollision();
  }


  public void AssignTarget(Transform target, float threat = -1f)
  {
    if (target == null) return;

    DefaultTarget = target;

    if (threat < 0f)
      threat = targetSelectorProfile.SpawnThreat;

    // ✅ force ใส่ threatTable ก่อน TickSelectTarget
    EnemyTargetSelector.RegisterThreat(target, threat, false);
    EnemyTargetSelector.TickSelectTarget();

    CrowdingTracker.Instance?.Unregister(CurrentTarget);
    CurrentTarget = EnemyTargetSelector.CurrentTarget;
    NavigationAgent.SetTarget(CurrentTarget);
    CrowdingTracker.Instance?.Register(CurrentTarget);

    if (_current == IdleState)
      ChangeState(ChaseState);
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

    Steering.obstacleDist = config.obstacleDist;
    Steering.obstacleStrength = config.obstacleStrength;
    Steering.avoidAngle = config.avoidAngle;

    Steering.separationRadius = config.separationRadius;
    Steering.separationStrength = config.separationStrength;
    Steering.enemyLayerMask = config.enemyLayer;

    Steering.cornerRadius = config.cornerRadius;
    Steering.cornerPush = config.cornerPush;
    Steering.passageProbeDist = config.passageProbeDist;
    Steering.narrowThreshold = config.narrowThreshold;
    Steering.centerStrength = config.centerStrength;
    Steering.narrowSpeedMul = config.narrowSpeedMul;
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

  #endregion

  // =============================
  // Lifecycle
  // =============================

  #region Lifecycle
  public override void OnSpawnFromPool(GameObject ob)
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
    Combat.OnNavigationPauseRequested += RequestNavigationPause;

    Combat.OnPlayAttack += HandleAttackAnimation;
    Combat.OnPlayPrepareDash += HandlePrepareDashAnimation;
    Combat.OnPlayDash += HandleDashAnimation;
    Combat.OnPlayEndDash += HandleEndDashAnimation;

    Combat.OnTargetDeath += HandleTargetDeath;

    Locomotion.OnVelocityChanged += HandleMovementDirectionAnimation;

    EnemyManager.Instance?.RegisterEnemy(this);
  }

  public override void OnReturnToPool(GameObject ob)
  {
    CrowdingTracker.Instance?.Unregister(CurrentTarget);

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

  public override bool TakeDamage(DamageContext context)
  {
    if (!Health.IsAlive)
      return true;

    FlashHitView?.FlashEffect();

    Health.TakeDamage(context.Damage);
    Locomotion.ApplyKnockback(
      context.HitDirection,
      context.KnockForce,
      context.KnockDration);

    bool isDead = !Health.IsAlive;

    var result = new CharacterDamageResult(
      context.Damage,
      transform.position,
      context.HitDirection,
      isDead);

    RaiseDamaged(result);

    if (isDead)
    {
      GiveReward(context);
      ChangeState(DeadState);
    }

    if (context.Source != null)
    {
      EnemyTargetSelector.RegisterThreat(
          context.Source,
          context.Damage * 3f,
          true
      );
    }

    return isDead;
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
    result.SourcePosition = transform.position;

    foreach (var stack in lootAll.loot)
      result.ItemRewards.Add(stack);

    OnGetLootable?.Invoke(result);
  }
  #endregion



  #region Tick
  private void Update()
  {
    if (_isMovementStopped && Time.time >= _stopUntilTime)
      _isMovementStopped = false;
  }

  private void FixedUpdate()
  {
    if (!Health.IsAlive) return;

    _current?.ManualFixedUpdate();

    if (_navigationPaused || _isMovementStopped)
    {
      Locomotion.StopMovement();
      return;
    }

    if (Steering.flowKey == null || !NavigationAgent.HasValidFlow)
    {
      Locomotion.StopMovement();
      return;
    }

    if (NavigationAgent.HasReachedTarget())
    {
      Locomotion.StopMovement();
      return;
    }

    SteeringResult result = Steering.TickSteering(FlowFieldOwner.Footprint);

    if (result.desiredDir.sqrMagnitude < 0.001f)
    {
      Vector2 escape = ComputeEscapeDir();
      if (escape.sqrMagnitude > 0.001f)
        result = new SteeringResult(escape, 0.5f, 1f);
    }

    Locomotion.ApplySteering(result);
  }

  // =============================
  // SENSOR SYSTEM
  // =============================

  private void TickSensor()
  {
    Sensor.ScanTargets();

    Debug.Log($"[Sensor] VisibleTargets count={Sensor.VisibleTargets.Count}");

    foreach (var target in Sensor.VisibleTargets)
    {
      Debug.Log($"[Sensor] sees={target.name}");
      float threat = 1f;

      var flowTarget = target.GetComponent<FlowFieldTarget>();
      if (flowTarget != null)
        threat = flowTarget.BaseThreat;

      threat += 1f / Mathf.Max(
       Vector2.Distance(transform.position, target.position), 0.5f);

      EnemyTargetSelector.RegisterThreat(target, threat, false);
    }

    if (_current == IdleState &&
        EnemyTargetSelector.CurrentTarget != null)
    {
      ChangeState(ChaseState);
    }
  }

  private void TickState()
  {
    EnemyTargetSelector.TickSelectTarget();

    var newTarget = EnemyTargetSelector.CurrentTarget;

    if (newTarget != CurrentTarget)
    {
      CrowdingTracker.Instance?.Unregister(CurrentTarget);
      CurrentTarget = newTarget;
      NavigationAgent.SetTarget(CurrentTarget);
      CrowdingTracker.Instance?.Register(CurrentTarget);
    }

    if (CurrentTarget != null)
    {
      var flowTarget = CurrentTarget.GetComponent<FlowFieldTarget>();
      if (flowTarget != null)
      {
        FlowFieldNavigationService.Instance.EnsureField(
            flowTarget.FlowKey,
            FlowFieldOwner.Footprint,
            CurrentTarget.position);
      }
    }

    if (_current == IdleState && CurrentTarget != null)
      ChangeState(ChaseState);

    _current?.ManualUpdate();
  }

  public void RequestNavigationPause(bool pause)
  {
    _navigationPaused = pause;
  }

  #endregion

  private static readonly List<Vector3Int> _escapeBuffer = new List<Vector3Int>();

  private Vector2 ComputeEscapeDir()
  {
    var grid = FlowFieldManager.Instance.world.GridConverter;
    FlowFieldOwner.GetFootprintCells(transform.position, grid, _escapeBuffer);

    Vector2 push = Vector2.zero;
    foreach (var cell in _escapeBuffer)
    {
      var state = FlowFieldManager.Instance.world.GetCell(cell);
      if (state != null && state.BlocksMovement)
      {
        Vector3 cellWorld = grid.GetCellCenterWorld(cell);
        push += (Vector2)(transform.position - cellWorld);
      }
    }

    return push.sqrMagnitude > 0.001f ? push.normalized : Vector2.zero;
  }

  public void OnTargetLost(Transform target)
  {
    EnemyTargetSelector.RemoveTarget(target);

    // ถ้าเป็น currentTarget → select ใหม่ทันที
    if (CurrentTarget == target)
    {
      CrowdingTracker.Instance?.Unregister(target);
      EnemyTargetSelector.TickSelectTarget();
      CurrentTarget = EnemyTargetSelector.CurrentTarget;
      NavigationAgent.SetTarget(CurrentTarget);
      CrowdingTracker.Instance?.Register(CurrentTarget);
    }
  }


  // =============================
  // STOP & DASH
  // =============================
  #region STOP & DASH
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
  #endregion

  // =============================
  // Animation
  // =============================
  #region Animation
  private void HandleMovementDirectionAnimation(Vector2 dir)
  {
    AnimationSystem.SetMoveDirection(dir);
    AnimationSystem.SetLookDirection(dir);
  }
  private void HandleAttackAnimation(string attackTag)
  {
    var tag = AnimationSystem.AnimationLibrary.AttackTag;
    if (tag == null) return;

    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection));
  }

  private void HandlePrepareDashAnimation()
  {
    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.PrepareDashTag;

    if (tag == null) return;

    AnimationSystem.Handle(new CharacterAnimationCommand(tag, dir));
  }

  private void HandleDashAnimation()
  {
    Debug.Log("Dash");

    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.DashTag;

    if (tag == null) return;

    AnimationSystem.Handle(new CharacterAnimationCommand(tag, dir));
  }

  private void HandleEndDashAnimation()
  {
    Debug.Log("EndDash");

    Vector2 dir = State.MoveDirection;

    var tag = AnimationSystem.AnimationLibrary.EndDashTag;

    if (tag == null) return;

    AnimationSystem.Handle(new CharacterAnimationCommand(tag, dir));
  }


  private void HandleTargetDeath(Transform target)
  {
    EnemyTargetSelector.RemoveTarget(target);

    EnemyTargetSelector.TickSelectTarget();
    CurrentTarget = EnemyTargetSelector.CurrentTarget;
    NavigationAgent.SetTarget(CurrentTarget);
  }
  #endregion
}