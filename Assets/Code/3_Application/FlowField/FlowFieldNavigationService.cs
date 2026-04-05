using System.Collections.Generic;
using UnityEngine;

public class FlowFieldNavigationService : MonoBehaviour
{
  public static FlowFieldNavigationService Instance;
  private Dictionary<FlowFieldChannelKey, Vector3> _lastTargets = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  public void EnsureField(FlowFieldChannelKey key, Vector3 targetPos)
  {
    var manager =
        FlowFieldManager.Instance;

    if (!manager.TryGetField(
        key,
        out var field))
    {
      manager.BuildField(
          key,
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
      manager.BuildField(
          key,
          targetPos);

      _lastTargets[key] =
          targetPos;
    }
  }
}
