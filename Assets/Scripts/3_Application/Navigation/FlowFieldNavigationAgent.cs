using UnityEngine;

public class FlowFieldNavigationAgent : INavigationAgent
{
    private readonly Transform _transform;
    private readonly FlowFieldOwner _flowFieldOwner;
    private readonly IFlowKeyHolder _steering;

    private Transform _target;

    public FlowFieldNavigationAgent(
        Transform ownerTransform,
        FlowFieldOwner flowFieldOwner,
        IFlowKeyHolder steering)
    {
        _transform = ownerTransform;
        _flowFieldOwner = flowFieldOwner;
        _steering = steering;
    }

    public bool HasValidFlow =>
        _steering.FlowKey != null
        && FlowFieldManager.Instance.TryGetField(
            _steering.FlowKey,
            _flowFieldOwner.Footprint,
            out _);

    public void SetTarget(Transform target)
    {
        _target = target;
        if (_target == null) return;

        var flowTarget = _target.GetComponent<FlowFieldTarget>();
        if (flowTarget == null) return;

        if (_steering.FlowKey == flowTarget.FlowKey) return;

        _steering.FlowKey = flowTarget.FlowKey;
        RequestFlowUpdateImmediate();
    }

    public void RequestFlowUpdateImmediate()
    {
        if (_target == null) return;
        if (HasReachedTarget()) return;

        var flowTarget = _target.GetComponent<FlowFieldTarget>();
        if (flowTarget == null) return;

        FlowFieldNavigationService.Instance.EnsureField(
            flowTarget.FlowKey,
            _flowFieldOwner.Footprint,
            _target.position);
    }

    public bool HasReachedTarget()
    {
        if (_target == null) return true;

        var grid = FlowFieldManager.Instance.world.GridConverter;
        float cellSize = grid.CellSize;

        float pivotToEdge = Mathf.Max(
            Mathf.Max(_flowFieldOwner.PivotAnchor.x, _flowFieldOwner.Footprint.x - 1 - _flowFieldOwner.PivotAnchor.x),
            Mathf.Max(_flowFieldOwner.PivotAnchor.y, _flowFieldOwner.Footprint.y - 1 - _flowFieldOwner.PivotAnchor.y)
        ) * cellSize;

        float targetRadius = _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
        float dist = Vector2.Distance(_transform.position, _target.position);

        return dist <= pivotToEdge + targetRadius + 0.1f;
    }
}
