using UnityEngine;

public readonly struct DirectInteractData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PointerPosition;
    public readonly InteractionTarget Target;

    public DirectInteractData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        InteractionTarget target = default)
    {
        ItemInstance = itemInstance;
        PointerPosition = pointerPosition;
        Target = target;
    }
}