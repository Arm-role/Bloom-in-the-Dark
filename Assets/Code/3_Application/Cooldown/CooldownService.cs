using System;
using System.Collections.Generic;

public sealed class CooldownService
{
  private readonly ITimeSource _time;
  private readonly Dictionary<object, CooldownContainer> _containers
      = new();

  public event Action<object, string> OnCooldownStarted;

  public CooldownService(ITimeSource time)
  {
    _time = time;
  }

  public CooldownContainer GetOrCreate(object owner)
  {
    if (!_containers.TryGetValue(owner, out var container))
    {
      container = new CooldownContainer(_time);
      container.OnCooldownStarted += key =>
                OnCooldownStarted?.Invoke(owner, key);

      _containers.Add(owner, container);
    }

    return container;
  }

  public float GetNormalized(object owner, string key)
  {
    if (_containers.TryGetValue(owner, out var container) &&
        container.TryGetCooldown(key, out var cd))
    {
      return cd.Normalized;
    }

    return 0f;
  }

  public bool RemoveOwner(object owner)
  {
    return _containers.Remove(owner);
  }

  public void ClearAll()
  {
    _containers.Clear();
  }
}
