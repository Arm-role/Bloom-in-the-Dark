using System;
using UnityEngine;

public sealed class NpcWalkToState : INpcState
{
    private readonly NpcController _c;
    private readonly Vector3 _destination;
    private readonly Action _onArrived;
    private readonly float _arrivalThreshold;

    public NpcWalkToState(NpcController c, Vector3 destination, Action onArrived, float arrivalThreshold)
    {
        _c = c;
        _destination = destination;
        _onArrived = onArrived;
        _arrivalThreshold = arrivalThreshold;
    }

    public void Enter()
    {
        _c.Steering.FlowKey = _c.PatrolChannel;
        FlowFieldNavigationService.Instance.EnsureField(
            _c.PatrolChannel,
            _c.FlowFieldOwner.Footprint,
            _destination);
    }

    public void Exit() { }

    public void Tick()
    {
        if (Vector2.Distance(_c.transform.position, _destination) <= _arrivalThreshold)
            _onArrived?.Invoke();
    }

    public void FixedTick()
    {
        if (!FlowFieldManager.Instance.TryGetField(
                _c.Steering.FlowKey, _c.FlowFieldOwner.Footprint, out _))
            return;

        SteeringResult result = _c.Steering.Sample();
        if (result.desiredDir.sqrMagnitude > 0.001f)
            _c.Locomotion.ApplySteering(result);
        else
            _c.Locomotion.Stop();
    }
}
