using System;
using UnityEngine;

[RequireComponent(typeof(FlowFieldOwner), typeof(NpcSteering), typeof(NpcLocomotion))]
public class NpcController : MonoBehaviour
{
  [Header("Visual")]
  [SerializeField] private Transform AnimationViewRoot;

  [Header("Patrol")]
  [SerializeField] private FlowFieldChannelKey _patrolChannel;
  [SerializeField] private float _patrolRadius = 5f;
  [SerializeField] private float _patrolWaitTime = 2f;
  [SerializeField] private float _arrivalDistance = 0.5f;
  [Tooltip("เดินถึงจุดแล้ว patrol ต่อ — ปิดสำหรับ NPC ที่ต้องยืนนิ่ง เช่นพ่อค้า")]
  [SerializeField] private bool _patrolAfterArrival = true;

  public FlowFieldChannelKey PatrolChannel => _patrolChannel;
  public float PatrolRadius => _patrolRadius;
  public float PatrolWaitTime => _patrolWaitTime;

  public FlowFieldOwner FlowFieldOwner { get; private set; }
  public NpcSteering Steering { get; private set; }
  public NpcLocomotion Locomotion { get; private set; }
  public INavigationAgent NavigationAgent { get; private set; }
  public ICharacterAnimationView AnimView { get; private set; }

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
    AnimView = AnimationViewRoot != null
        ? AnimationViewRoot.GetComponent<ICharacterAnimationView>()
        : null;

    NavigationAgent = new FlowFieldNavigationAgent(transform, FlowFieldOwner, Steering);

    IdleState = new NpcIdleState(this);
    FollowState = new NpcFollowState(this);
    PatrolState = new NpcPatrolState(this, transform.position);
  }

  private void Start()
  {
    if (_current == null)
      ChangeState(IdleState);
  }

  public void StartPatrol()
  {
    PatrolState = new NpcPatrolState(this, transform.position);
    ChangeState(PatrolState);
  }

  public void WalkToThenPatrol(Vector3 destination)
  {
    // NPC ที่ปิด patrol (เช่นพ่อค้า) เดินถึงจุดแล้วยืนนิ่งที่ Idle
    Action onArrived = StartPatrol;
    if (!_patrolAfterArrival)
      onArrived = () => ChangeState(IdleState);

    ChangeState(new NpcWalkToState(this, destination, onArrived, _arrivalDistance));
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

    AnimView?.SetMoveDirection(Locomotion.Velocity.normalized);

    if (Input.GetKeyDown(KeyCode.P))
    {
      if (_current == PatrolState)
        ChangeState(IdleState);
      else
        ChangeState(PatrolState);
    }
  }

  private void FixedUpdate()
  {
    _current?.FixedTick();
  }
}
