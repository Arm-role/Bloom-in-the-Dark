using System;
using System.Collections.Generic;

public sealed class CooldownContainer
{
  private readonly Dictionary<string, CooldownData> _cooldowns = new();
  private readonly ITimeSource _time;

  public event Action<string> OnCooldownStarted;

  public CooldownContainer(ITimeSource time)
  {
    _time = time;
  }

  public bool IsOnCooldown(string key)
  {
    return _cooldowns.TryGetValue(key, out var cd)
           && cd.IsActive;
  }

  public bool TryApply(string key, float duration)
  {
    if (duration <= 0f)
      return false;

    if (IsOnCooldown(key))
      return false;

    _cooldowns[key] = new CooldownData(
        _time.Now,
        duration,
        _time);

    OnCooldownStarted?.Invoke(key);

    return true;
  }
  public bool TryGetCooldown(string key, out CooldownData cooldown)
  {
    if (_cooldowns.TryGetValue(key, out cooldown))
      return cooldown.IsActive;

    cooldown = default;
    return false;
  }

  public float GetNormalized(string key)
  {
    if (TryGetCooldown(key, out var cd))
    {
      return cd.Normalized;
    }

    return 0f;
  }

  public void Clear(string key)
  {
    _cooldowns.Remove(key);
  }
}