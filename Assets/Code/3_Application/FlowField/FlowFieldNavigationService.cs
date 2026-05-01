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

  public void EnsureField(FlowFieldChannelKey channel, Vector2Int footprint, Vector3 targetPos)
  {
    var manager = FlowFieldManager.Instance;
    FlowFieldKey key = new FlowFieldKey(channel, footprint);

    bool hasField = manager.TryGetField(channel, footprint, out _);

    if (_lastTargets.TryGetValue(key, out var last))
    {
      // target ไม่ขยับ → ไม่ rebuild
      if (hasField && Vector3.Distance(last, targetPos) <= 0.5f)
        return;
    }

    manager.RemoveField(channel, footprint);
    manager.BuildField(channel, footprint, targetPos);
    _lastTargets[key] = targetPos;
  }
}
