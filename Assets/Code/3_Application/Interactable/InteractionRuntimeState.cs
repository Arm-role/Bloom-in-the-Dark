using System.Collections.Generic;
using UnityEngine;

public class InteractionRuntimeState
{
  private readonly Dictionary<string, float> _cooldowns = new();

  public bool IsReady(string key)
  {
    return !_cooldowns.TryGetValue(key, out var endTime)
           || Time.time >= endTime;
  }

  public void SetCooldown(string key, float duration)
  {
    _cooldowns[key] = Time.time + duration;
  }
}