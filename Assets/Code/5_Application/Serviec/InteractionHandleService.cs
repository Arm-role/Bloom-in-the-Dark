using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandleService
{
    private readonly Dictionary<EItemStrategyType, ItemStrategyBundle> _strategies
        = new();

    public void Register(EItemStrategyType strategyType, ItemStrategyBundle bundle)
    {
        if (bundle == null)
        {
            Debug.LogWarning($"[InteractionHandleService] Invalid bundle registration for {strategyType}");
            return;
        }

        _strategies[strategyType] = bundle;
    }

    public ItemStrategyBundle Resolve(EItemStrategyType strategyType)
    {
        if (_strategies.TryGetValue(strategyType, out var bundle))
            return bundle;

        Debug.LogWarning($"[InteractionHandleService] No strategy found for {strategyType}");
        return null;
    }
}