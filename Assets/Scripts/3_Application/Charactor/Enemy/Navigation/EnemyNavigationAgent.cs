using UnityEngine;

public class EnemyNavigationAgent : INavigationAgent
{
    private readonly FlowFieldNavigationAgent _inner;

    public EnemyNavigationAgent(EnemyController controller)
    {
        _inner = new FlowFieldNavigationAgent(
            controller.transform,
            controller.FlowFieldOwner,
            controller.Steering
        );
    }

    public bool HasValidFlow => _inner.HasValidFlow;
    public void SetTarget(Transform target) => _inner.SetTarget(target);
    public bool HasReachedTarget() => _inner.HasReachedTarget();
    public void RequestFlowUpdateImmediate() => _inner.RequestFlowUpdateImmediate();
}
