using UnityEngine;

[RequireComponent(typeof(EnemyMovement), typeof(EnemySensor))]
public class EnemyController : MonoBehaviour
{
    public Transform Player;
    public LayerMask playerMask;

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

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Sensor = GetComponent<EnemySensor>();
        Combat = gameObject.AddComponent<EnemyCombat>();
        Data = new EnemyData(transform);
        AnimView = GetComponent<ICharacterAnimationView>();

        // State creation
        IdleState = new IdleState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
        DeadState = new DeadState(this);

        // Combat → Animation & Movement
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
    }

    public void Initialize(Transform player, float moveSpeed = 3f, int hp = 10)
    {
        Player = player;

        Movement.speed = moveSpeed;
        Data.MoveSpeed = moveSpeed;
        Data.MaxHP = hp;
        Data.CurrentHP = hp;

        Combat.Initialize(player);

        // register enemy
        EnemyManager.Instance?.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        EnemyManager.Instance?.UnregisterEnemy(this);
    }

    private void Update()
    {
        ManualUpdate();
    }

    private void FixedUpdate()
    {
        ManualFixedUpdate();
    }

    // ===========================================================
    //                          UPDATE
    // ===========================================================
    public void ManualUpdate()
    {
        if (Data.IsDead) return;

        // let state update logic execute
        _current?.ManualUpdate();

        // handle movement stop timer
        if (_isMovementStopped && Time.time >= _stopUntilTime)
            _isMovementStopped = false;
    }

    public void ManualFixedUpdate()
    {
        if (Data.IsDead) return;

        // global movement stop
        if (_isMovementStopped)
        {
            Movement.Stop();
            return;
        }

        // delegate physics movement to state
        _current?.ManualFixedUpdate();
    }

    // ===========================================================
    //                        STATE MACHINE
    // ===========================================================
    public void ChangeState(IEnemyState newState)
    {
        if (_current == newState) return;

        _current?.Exit();
        _current = newState;
        _current?.Enter();
    }

    // ===========================================================
    //                          COMBAT
    // ===========================================================
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

    private System.Collections.IEnumerator StopAfterDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        Movement.Stop();
    }

    private void OnDied()
    {
        ChangeState(DeadState);
        Movement.Stop();
    }

    public void TakeDamage(int amount)
    {
        if (Data.IsDead) return;

        Data.TakeDamage(amount);
        AnimView?.PlayHit();
    }

    public void AddSkill(IEnemySkill skill)
    {
        Combat.AddSkill(skill);
    }
}