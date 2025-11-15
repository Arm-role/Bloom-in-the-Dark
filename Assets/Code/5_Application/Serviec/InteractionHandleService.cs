using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractionHandleService
{
    private readonly Dictionary<(EItemType, EItemStategyType), ItemStrategyBundle> _localStrategies
         = new();

    private ItemStrategyBundle _globalStrategyBundle;

    public void SetGlobal(ItemStrategyBundle globalStrategyBundle)
    {
        _globalStrategyBundle = globalStrategyBundle;
    }
    public void Register(EItemType itemType, EItemStategyType strategyType, ItemStrategyBundle bundle)
    {
        var key = (itemType, strategyType);

        if (bundle == null)
        {
            Debug.LogWarning($"[InteractionHandleService] Invalid bundle registration for {key}");
            return;
        }

        _localStrategies[key] = bundle;
    }

    public ItemStrategyBundle Resolve(EItemType itemType, EItemStategyType strategyType)
    {
        var key = (itemType, strategyType);

        if (_localStrategies.TryGetValue(key, out var bundle))
            return bundle;

        Debug.LogWarning($"[InteractionHandleService] No strategy found for {key}");
        return null;
    }

    public ItemStrategyBundle GetGlobal()
    {
        return _globalStrategyBundle;
    }
}
