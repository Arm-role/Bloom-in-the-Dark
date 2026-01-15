using UnityEngine;

public readonly struct InteractionHandleContext
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PlayerPosition;
    public readonly Vector2? PointerPosition;
    public readonly Vector2? PlayerDirection;
    public readonly InputActionType InputActionType;
    public readonly EInteractionIntentType InteractionType;

    public InteractionHandleContext(
        IItemInstance itemInstance = null,
        Vector2? playerPosition = null,
        Vector2? pointerPosition = null, 
        Vector2? playerDirection = null,
        InputActionType inputActionType = InputActionType.None,
        EInteractionIntentType intentType = EInteractionIntentType.None)
    {
        ItemInstance = itemInstance;
        PlayerPosition = playerPosition;
        PointerPosition = pointerPosition;
        PlayerDirection = playerDirection;
        InputActionType = inputActionType;
        InteractionType = intentType;
    }
    public InteractionIntent ToIntent()
    {
        return new InteractionIntent(
            type: InteractionType,
            sourceItem: ItemInstance,
            input: InputActionType,
            worldPosition: PointerPosition ?? Vector2.zero,
            pointerPosition: PointerPosition,
            direction: PlayerDirection ??  Vector2.zero
        );
    }
}
