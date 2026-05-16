using UnityEngine;

[RequireComponent(typeof(FlowFieldOwner), typeof(NpcSteering), typeof(NpcLocomotion))]
public class NpcController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private FlowFieldChannelKey _patrolChannel;
    [SerializeField] private float _patrolRadius = 5f;
    [SerializeField] private float _patrolWaitTime = 2f;

    public FlowFieldChannelKey PatrolChannel => _patrolChannel;
    public float PatrolRadius => _patrolRadius;
    public float PatrolWaitTime => _patrolWaitTime;

    public FlowFieldOwner FlowFieldOwner { get; private set; }
    public NpcSteering Steering { get; private set; }
    public NpcLocomotion Locomotion { get; private set; }
    public INavigationAgent NavigationAgent { get; private set; }

    public Transform CurrentTarget { get; private set; }

    public INpcState IdleState { get; private set; }
    public INpcState FollowState { get; private set; }
    public INpcState PatrolState { get; private set; }

    private INpcState _current;

    private void Awake()
    {
        FlowFieldOwner = GetComponent<FlowFieldOwner>();
        Steering = GetComponent<NpcSteering>();
        Locomotion = GetComponent<NpcLocomotion>();

        NavigationAgent = new FlowFieldNavigationAgent(transform, FlowFieldOwner, Steering);

        IdleState = new NpcIdleState(this);
        FollowState = new NpcFollowState(this);
        PatrolState = new NpcPatrolState(this, transform.position);
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    // เรียกจากภายนอก เช่น installer หรือ quest system
    public void StartPatrol()
    {
        ChangeState(PatrolState);
    }

    public void AssignTarget(Transform target)
    {
        if (target == null) return;
        CurrentTarget = target;
        NavigationAgent.SetTarget(target);
        ChangeState(FollowState);
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
        ChangeState(IdleState);
    }

    public void ChangeState(INpcState newState)
    {
        if (_current == newState) return;
        _current?.Exit();
        _current = newState;
        _current?.Enter();
    }

    private void Update()
    {
        _current?.Tick();
    }

    private void FixedUpdate()
    {
        _current?.FixedTick();
    }
}
