using System.Collections.Generic;
using UnityEngine;

public sealed class ItemCooldownComponent : ICooldownOwner
{
    private readonly Dictionary<string, CooldownData> _cooldowns
        = new();

    public bool IsOnCooldown(string key)
    {
        return _cooldowns.TryGetValue(key, out var cd)
               && cd.IsActive;
    }

    public bool TryGetCooldown(
        string key,
        out CooldownData cooldown)
    {
        if (_cooldowns.TryGetValue(key, out cooldown))
            return cooldown.IsActive;

        cooldown = default;
        return false;
    }

    public void ApplyCooldown(
        string key,
        float duration)
    {
        if (duration <= 0f) return;

        _cooldowns[key] = new CooldownData(
            Time.time,
            duration);
    }

    public void Clear(string key)
    {
        _cooldowns.Remove(key);
    }
}