using System.Collections.Generic;
using UnityEngine;

public class FlowFieldNavigationService : MonoBehaviour
{
  public static FlowFieldNavigationService Instance;

  private readonly Dictionary<FlowFieldKey, Vector3> _lastTargets = new();

  private struct PendingBuild
  {
    public FlowFieldChannelKey channel;
    public Vector2Int footprint;
    public Vector3 targetPos;
  }

  private readonly Dictionary<FlowFieldKey, PendingBuild> _pendingRebuild = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  private void LateUpdate()
  {
    FlushPendingRebuild();
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

    if (!hasField)
    {
      BuildImmediate(manager, key, channel, footprint, targetPos);
      return;
    }

    if (_lastTargets.TryGetValue(key, out var last) &&
        Vector3.Distance(last, targetPos) <= rebuildThreshold)
      return;

    _pendingRebuild[key] = new PendingBuild
    {
      channel = channel,
      footprint = footprint,
      targetPos = targetPos
    };
  }

  private void FlushPendingRebuild()
  {
    if (_pendingRebuild.Count == 0) return;

    var manager = FlowFieldManager.Instance;

    foreach (var kv in _pendingRebuild)
    {
      var p = kv.Value;
      BuildImmediate(manager, kv.Key, p.channel, p.footprint, p.targetPos);
    }

    _pendingRebuild.Clear();
  }

  private void BuildImmediate(
      FlowFieldManager manager,
      FlowFieldKey key,
      FlowFieldChannelKey channel,
      Vector2Int footprint,
      Vector3 targetPos)
  {
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
