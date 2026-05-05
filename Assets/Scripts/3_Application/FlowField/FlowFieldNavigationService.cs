using System.Collections.Generic;
using UnityEngine;

public class FlowFieldNavigationService : MonoBehaviour
{
  public static FlowFieldNavigationService Instance;
  private Dictionary<FlowFieldKey, Vector3> _lastTargets = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  public void EnsureField(
     FlowFieldChannelKey channel,
     Vector2Int footprint,
     Vector3 targetPos,
     float rebuildThreshold = 1.5f)
  {
    var manager = FlowFieldManager.Instance;
    FlowFieldKey key = new FlowFieldKey(channel, footprint);

    bool hasField = manager.TryGetField(channel, footprint, out _);

    if (hasField && _lastTargets.TryGetValue(key, out var last))
    {
      if (Vector3.Distance(last, targetPos) <= rebuildThreshold)
        return;
    }

    var reachable = manager.FindClosestReachableCells(
        targetPos,
        footprint,
        Vector2Int.zero
    );

    var targets = reachable.Count > 0 ? reachable : new List<Vector3> { targetPos };

    manager.RemoveField(channel, footprint);
    manager.BuildField(channel, footprint, targets);
    _lastTargets[key] = targetPos;
  }
}
