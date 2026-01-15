using UnityEngine;

public readonly struct InteractionIntent
{
    public readonly EInteractionIntentType Type;

    // ใครทำ (tool / item / มือเปล่า)
    public readonly IItemInstance SourceItem;

    // input context
    public readonly InputActionType Input;
    public readonly Vector2 WorldPosition;
    public readonly Vector2? PointerPosition;
    public readonly Vector2 Direction;

    public InteractionIntent(
        EInteractionIntentType type,
        IItemInstance sourceItem,
        InputActionType input,
        Vector2 worldPosition,
        Vector2? pointerPosition,
        Vector2 direction)
    {
        Type = type;
        SourceItem = sourceItem;
        Input = input;
        WorldPosition = worldPosition;
        PointerPosition = pointerPosition;
        Direction = direction;
    }

    public InteractionIntent WithType(EInteractionIntentType type)
    => new InteractionIntent(
        type: type,
        sourceItem: SourceItem,
        input: Input,
        worldPosition: WorldPosition,
        pointerPosition: PointerPosition,
        direction: Direction);
    
    public bool HasItem => SourceItem != null;

    public T GetItem<T>() where T : class
        => SourceItem?.Data as T;
}
public static class InteractionIntentExtensions
{
    public static bool Is(
        this InteractionIntent intent,
        params EInteractionIntentType[] types)
    {
        foreach (var t in types)
            if (intent.Type == t)
                return true;
        return false;
    }
}
