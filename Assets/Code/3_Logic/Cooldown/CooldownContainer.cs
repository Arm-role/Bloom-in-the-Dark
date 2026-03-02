using System;
using System.Collections.Generic;

public sealed class CooldownContainer
{
  private readonly Dictionary<string, CooldownData> _cooldowns = new();

  private readonly HashSet<string> _activeKeys = new();
  public IReadOnlyCollection<string> ActiveKeys => _activeKeys;

  private readonly ITimeSource _time;

  public event Action<string> OnCooldownStarted;
  public event Action<string> OnCooldownEnded;
  public CooldownContainer(ITimeSource time)
  {
    _time = time;
  }

  public void Tick()
  {
    if (_activeKeys.Count == 0)
      return;

    var finished = new List<string>();

    foreach (var key in _activeKeys)
    {
      if (!_cooldowns.TryGetValue(key, out var cd))
      {
        finished.Add(key);
        continue;
      }

      if (!cd.IsActive)
        finished.Add(key);
    }

    foreach (var key in finished)
    {
      _cooldowns.Remove(key);
      _activeKeys.Remove(key);
      OnCooldownEnded?.Invoke(key);
    }
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

    var data = new CooldownData(
        _time.Now,
        duration,
        _time);

    _cooldowns[key] = data;
    _activeKeys.Add(key);

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
  public float GetCooldown(string key)
  {
    if (TryGetCooldown(key, out var cd))
    {
      return cd.Remaining;
    }

    return 0f;
  }

  public void Clear(string key)
  {
    _cooldowns.Remove(key);
  }
}