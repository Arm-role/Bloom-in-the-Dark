using UnityEngine;

public readonly struct DirectInteractData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PointerPosition;
    public readonly InteractionTargetContext Target;

    public DirectInteractData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        InteractionTargetContext target = default)
    {
        ItemInstance = itemInstance;
        PointerPosition = pointerPosition;
        Target = target;
    }
}