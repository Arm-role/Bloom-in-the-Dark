using System.Collections.Generic;
using UnityEngine;

public class InteractionHandleService
{
    private readonly Dictionary<(EItemType, EItemStategyType), ItemStrategyBundle> _strategies
         = new();

    public void Register(EItemType itemType, EItemStategyType strategyType, ItemStrategyBundle bundle)
    {
        var key = (itemType, strategyType);

        if (bundle == null)
        {
            Debug.LogWarning($"[InteractionHandleService] Invalid bundle registration for {key}");
            return;
        }

        _strategies[key] = bundle;
    }

    public ItemStrategyBundle Resolve(EItemType itemType, EItemStategyType strategyType)
    {
        var key = (itemType, strategyType);

        if (_strategies.TryGetValue(key, out var bundle))
            return bundle;

        Debug.LogWarning($"[InteractionHandleService] No strategy found for {key}");
        return null;
    }
}
