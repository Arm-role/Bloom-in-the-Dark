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
    var manager =
        FlowFieldManager.Instance;

    FlowFieldKey key = new FlowFieldKey(channel, footprint);

    if (!manager.TryGetField(
        channel,
        footprint,
        out var field))
    {
      manager.BuildField(
          channel, 
          footprint,
          targetPos);

      _lastTargets[key] =
          targetPos;

      return;
    }

    if (!_lastTargets.TryGetValue(
        key,
        out var last))
    {
      _lastTargets[key] =
          targetPos;

      return;
    }

    float dist =
        Vector3.Distance(
            last,
            targetPos);

    if (dist > 0.5f)
    {
      manager.RemoveField(
        channel,
        footprint
      );

      manager.BuildField(
          channel, 
          footprint,
          targetPos);

      _lastTargets[key] =
          targetPos;
    }
  }
}
