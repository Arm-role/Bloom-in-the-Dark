using UnityEngine;

public interface INavigationAgent
{
    void SetTarget(Transform target);
    bool HasValidFlow { get; }
    bool HasReachedTarget();
    void RequestFlowUpdateImmediate();
}
