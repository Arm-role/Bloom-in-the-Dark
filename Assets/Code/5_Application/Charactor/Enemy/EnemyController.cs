using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement), typeof(EnemySensor))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform Player;

    [Header("Debug")]
    public bool debugDraw = false;

    // Components
    public EnemyMovement Movement { get; private set; }
    public EnemySensor Sensor { get; private set; }
    public EnemyCombat Combat { get; private set; }
    public EnemyData Data { get; private set; }
    public ICharacterAnimationView AnimView { get; private set; }

    // States
    public IEnemyState IdleState { get; private set; }
    public IEnemyState ChaseState { get; private set; }
    public IEnemyState AttackState { get; private set; }
    public IEnemyState DeadState { get; private set; }

    private IEnemyState _current;

    // Movement stop control
    private bool _isMovementStopped = false;
    private float _stopUntilTime = 0f;

    // ticks
    private int _sensorTickId = -1;
    private int _stateTickId = -1;

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Sensor = GetComponent<EnemySensor>();
        Combat = gameObject.AddComponent<EnemyCombat>();
        Data = new EnemyData(transform);
        AnimView = GetComponent<ICharacterAnimationView>();

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

        Data.OnDied += OnDied;
    }

    private void Start()
    {
        ChangeState(IdleState);

        if (AITickManager.Instance != null)
        {
            _sensorTickId = AITickManager.Instance.Register(TickSensor, 8f);
            _stateTickId = AITickManager.Instance.Register(TickState, 15f);
        }

        EnemyManager.Instance?.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        EnemyManager.Instance?.UnregisterEnemy(this);

        if (AITickManager.Instance != null)
        {
            if (_sensorTickId >= 0) AITickManager.Instance.Unregister(_sensorTickId);
            if (_stateTickId >= 0) AITickManager.Instance.Unregister(_stateTickId);
        }
    }

    private void Update() => ManualUpdate();
    private void FixedUpdate()
    {
        ManualFixedUpdate();

        // --- NEW: stuck → let ChaseState re-evaluate immediately ---
        if (Movement.StuckJustTriggered)
        {
            // Force state update RIGHT NOW (instead of waiting 15 fps tick)
            _current?.ManualUpdate();
        }

        Movement.FollowPath();
    }
    public void ManualUpdate()
    {
        if (Data.IsDead) return;
        if (_isMovementStopped && Time.time >= _stopUntilTime) _isMovementStopped = false;
    }

    public void ManualFixedUpdate()
    {
        if (Data.IsDead) return;

        if (_isMovementStopped)
        {
            Movement.Stop();
            Movement.ClearPath(); // NEW
            return;
        }

        if (Movement.HasPath)
        {
            return;
        }

        _current?.ManualFixedUpdate();
    }

    private void TickSensor()
    {
        if (Data.IsDead) return;
        if (Player == null) return;

        Sensor.CheckDetect(Player);
        if (_current == IdleState && Sensor.DetectedTarget != null)
            ChangeState(ChaseState);
    }

    private void TickState()
    {
        if (Data.IsDead) return;
        _current?.ManualUpdate();
    }

    public void ChangeState(IEnemyState newState)
    {
        if (_current == newState) return;
        _current?.Exit();
        _current = newState;
        _current?.Enter();
    }

    public void Initialize(Transform player, float moveSpeed = 3f, int hp = 10)
    {
        Player = player;
        Movement.speed = moveSpeed;
        Data.MoveSpeed = moveSpeed;
        Data.MaxHP = hp;
        Data.CurrentHP = hp;
        Combat.Initialize(player);
    }

    public void TakeDamage(int amount)
    {
        if (Data.IsDead) return;
        Data.TakeDamage(amount);
        AnimView?.PlayHit();
    }

    public void AddSkill(IEnemySkill skill) => Combat.AddSkill(skill);

    private void OnRequestStopMovement(float duration)
    {
        _isMovementStopped = true;
        _stopUntilTime = Time.time + duration;
        Movement.Stop();
    }

    private void OnRequestDash(Vector2 impulse, float duration)
    {
        Movement.ApplyImpulse(impulse, duration);
        Combat.OnPlayDash?.Invoke();
        StopCoroutine(nameof(StopAfterDash));
        StartCoroutine(StopAfterDash(duration));
    }

    private IEnumerator StopAfterDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        Movement.Stop();
    }

    private void OnDied()
    {
        ChangeState(DeadState);
        Movement.Stop();
    }
}