using System.Collections.Generic;
using UnityEngine;

public class InteractionHandleService
{
    private readonly Dictionary<(EItemType, EItemStategyType), IInteractionHandle> _handlerMap
         = new Dictionary<(EItemType, EItemStategyType), IInteractionHandle>();

    private readonly Dictionary<(EItemType, EItemStategyType), IItemAction> _actionMap
        = new Dictionary<(EItemType, EItemStategyType), IItemAction>();

    public void Register(EItemType type, EItemStategyType stategyType, IInteractionHandle handler, IItemAction action)
    {
        var key = (type, stategyType);

        if (handler != null)
            _handlerMap[key] = handler;

        if (action != null)
            _actionMap[key] = action;
    }

    public (IInteractionHandle handler, IItemAction action)? GetHandler(EItemType type, EItemStategyType stategyType)
    {
        var key = (type, stategyType);

        _handlerMap.TryGetValue(key, out var handler);
        _actionMap.TryGetValue(key, out var action);

        if (handler == null && action == null)
        {
            Debug.LogWarning($"No handler or action registered for ({type}, {stategyType})");
            return null;
        }

        return (handler, action);
    }
}
