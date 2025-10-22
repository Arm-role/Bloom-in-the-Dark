using System.Collections.Generic;
using UnityEngine;

public class PreviewService 
{
    private readonly Dictionary<(EItemType,EItemStategyType), IInteractionHandle> _handlerMap
        = new Dictionary<(EItemType, EItemStategyType), IInteractionHandle>();

    public void Register(EItemType type, EItemStategyType stategyType, IInteractionHandle handler)
    {
        _handlerMap[(type, stategyType)] = handler;
    }

    public IInteractionHandle GetHandler(EItemType type, EItemStategyType stategyType)
    {
        if (_handlerMap.TryGetValue((type, stategyType), out var handler))
            return handler;

        Debug.LogWarning($"No preview handler registered for {type}");
        return null;
    }
}