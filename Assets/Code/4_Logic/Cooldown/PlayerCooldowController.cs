using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerCooldownController : ICooldownOwner
{
    private readonly Dictionary<string, CooldownData> _cooldowns = new();

    // ---------- Query ----------

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

    // ---------- Apply ----------

    public void ApplyCooldown(string key, float duration)
    {
        if (duration <= 0f) return;

        _cooldowns[key] = new CooldownData
        (
            Time.time,
            duration
        );
    }

    // ---------- Cleanup (optional) ----------

    public void Tick()
    {
        var removeKeys = ListPool<string>.Get();

        foreach (var kv in _cooldowns)
            if (!kv.Value.IsActive)
                removeKeys.Add(kv.Key);

        foreach (var k in removeKeys)
            _cooldowns.Remove(k);

        ListPool<string>.Release(removeKeys);
    }
}