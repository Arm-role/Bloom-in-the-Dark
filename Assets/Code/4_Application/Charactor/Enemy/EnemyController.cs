using System;
using UnityEngine;

[RequireComponent(
    typeof(EnemySteering),
    typeof(EnemyLocomotion),
    typeof(EnemySensor))]
public class EnemyController : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
    [Header("Cofig")] public EnemyConfig config;

    [Header("References")] public Transform Player;

    public EnemyLocomotion Locomotion { get; private set; }
    public EnemySteering Steering { get; private set; }
    public EnemySensor Sensor { get; private set; }
    public EnemyCombat Combat { get; private set; }
    public EnemyState State { get; private set; }
    public HealthResource Health { get; private set; }
    public EnemyTargetSelector EnemyTargetSelector { get; private set; }

    public ICharacterAnimationView AnimView { get; private set; }
    public IBarView HealthBarView { get; private set; }
    public IFlashHitView FlashHitView { get; private set; }

    public IEnemyState IdleState { get; private set; }
    public IEnemyState ChaseState { get; private set; }
    public IEnemyState AttackState { get; private set; }
    public IEnemyState DeadState { get; private set; }

    private BarPresenter<HealthResource> _healthPresenter;
    private Collider2D[] _collider2Ds;

    private IEnemyState _current;

    private bool _isMovementStopped = false;
    private float _stopUntilTime = 0f;

    private bool _holdPosition = false;

    private int _sensorTickId = -1;
    private int _stateTickId = -1;

    public bool IsAlive { get; set; }
    public event Action<GameObject> OnRequestDestruction;

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

        AnimView = GetComponent<ICharacterAnimationView>();
        FlashHitView = GetComponent<IFlashHitView>();
        HealthBarView = GetComponent<IBarView>();

        _collider2Ds = GetComponents<Collider2D>();

        IdleState = new IdleState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
        DeadState = new DeadState(this);

        Combat.OnPlayAttack += key => AnimView?.PlayAttack(key);
        Combat.OnPlayDash += () => AnimView?.PlayDash();
        Combat.OnPlaySlam += () => AnimView?.PlaySlam();
        Combat.OnPlayHit += () => AnimView?.PlayHit();
        Combat.OnRequestStopMovement += OnRequestStopMovement;
        Combat.OnRequestDash += OnRequestDash;
        Combat.OnRequestHoldPosition += OnRequestHoldPosition;
        
    }

    // ======================================================
    // POOL — full setup here
    // ======================================================

    public void Initialize(Transform player, float moveSpeed = 3f, int hp = 10)
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

        _current = null;
        ChangeState(IdleState);

        if (AITickManager.Instance != null)
        {
            _sensorTickId = AITickManager.Instance.Register(TickSensor, 8f);
            _stateTickId = AITickManager.Instance.Register(TickState, 15f);
        }

        EnemyManager.Instance?.RegisterEnemy(this);
    }

    public void OnReturnToPool(GameObject ob)
    {
        if (AITickManager.Instance != null)
        {
            if (_sensorTickId >= 0) AITickManager.Instance.Unregister(_sensorTickId);
            if (_stateTickId >= 0) AITickManager.Instance.Unregister(_stateTickId);
        }

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

    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }

    public void TakeDamage(
        float damage,
        Vector2 hitDir,
        float force,
        float duration)
    {
        if (!Health.IsAlive)
            return;

        FlashHitView?.FlashEffect();
        AnimView?.PlayHit();

        Health.TakeDamage(damage);
        Locomotion.ApplyKnockback(hitDir, force, duration);

        if (!Health.IsAlive)
            ChangeState(DeadState);
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
        Debug.Log("OnRequestDash");

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
}