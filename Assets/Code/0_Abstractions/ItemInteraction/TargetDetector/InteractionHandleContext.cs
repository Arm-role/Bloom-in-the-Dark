using UnityEngine;

public readonly struct InteractionHandleContext
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PlayerPosition;
    public readonly Vector2? PointerPosition;
    public readonly Collider2D Target;

    public InteractionHandleContext(
        IItemInstance itemInstance = null,
        Vector2? playerPosition = null,
        Vector2? pointerPosition = null, 
        Collider2D target = null)
    {
        ItemInstance = itemInstance;
        PlayerPosition = playerPosition;
        PointerPosition = pointerPosition;
        Target = target;
    }
}
