using UnityEngine;

public readonly struct InteractionHandleContext
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PlayerPosition;
    public readonly Vector2? PointerPosition;
    public readonly Vector2? PlayerDirection;
    public readonly InputActionType InputActionType;

    public InteractionHandleContext(
        IItemInstance itemInstance = null,
        Vector2? playerPosition = null,
        Vector2? pointerPosition = null,
        Vector2? playerDirection = null,
        InputActionType inputActionType = InputActionType.None)
    {
        ItemInstance = itemInstance;
        PlayerPosition = playerPosition;
        PointerPosition = pointerPosition;
        PlayerDirection = playerDirection;
        InputActionType = inputActionType;
    }

    public InteractionIntent ToIntent(EInteractionIntentType type)
    {
        return new InteractionIntent(
            type: type,
            sourceItem: ItemInstance,
            input: InputActionType,
            origin: PlayerPosition,
            direction: PointerPosition - PlayerPosition
        );
    }
}