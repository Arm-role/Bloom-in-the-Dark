using UnityEngine;

// NPC เดินเล่นใน area แบบ random — ไม่ต้องการ FlowFieldTarget บน waypoint
// ขับ FlowField โดยตรงผ่าน FlowFieldNavigationService + patrolChannel ของตัวเอง
public class NpcPatrolState : INpcState
{
    private readonly NpcController _c;
    private readonly Vector3 _areaCenter;

    private Vector3 _destination;
    private bool _waiting;
    private float _waitTimer;
    private float _destinationTimeout;

    private const float ARRIVE_THRESHOLD = 0.4f;
    private const float DESTINATION_TIMEOUT = 5f;

    public NpcPatrolState(NpcController c, Vector3 areaCenter)
    {
        _c = c;
        _areaCenter = areaCenter;
    }

    public void Enter()
    {
        _waiting = false;
        PickNextDestination();
    }

    public void Exit()
    {
        _waiting = false;
    }

    public void Tick()
    {
        if (_waiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _waiting = false;
                PickNextDestination();
            }
            return;
        }

        _destinationTimeout -= Time.deltaTime;

        float dist = Vector2.Distance(_c.transform.position, _destination);
        if (dist <= ARRIVE_THRESHOLD || _destinationTimeout <= 0f)
        {
            _waiting = true;
            _waitTimer = _c.PatrolWaitTime;
        }
    }

    public void FixedTick()
    {
        if (_waiting)
        {
            _c.Locomotion.Stop();
            return;
        }

        SteeringResult result = _c.Steering.Sample();
        if (result.desiredDir.sqrMagnitude > 0.001f)
            _c.Locomotion.ApplySteering(result);
        else
            _c.Locomotion.Stop();
    }

    private void PickNextDestination()
    {
        Vector2 rand = Random.insideUnitCircle * _c.PatrolRadius;
        Vector3 rawPos = _areaCenter + new Vector3(rand.x, rand.y, 0f);

        var grid = FlowFieldManager.Instance.world.GridConverter;
        _destination = grid.GridToWorld(grid.WorldToGrid(rawPos));

        _destinationTimeout = DESTINATION_TIMEOUT;

        FlowFieldNavigationService.Instance.EnsureField(
            _c.PatrolChannel,
            _c.FlowFieldOwner.Footprint,
            _destination);

        _c.Steering.FlowKey = _c.PatrolChannel;
    }
}
