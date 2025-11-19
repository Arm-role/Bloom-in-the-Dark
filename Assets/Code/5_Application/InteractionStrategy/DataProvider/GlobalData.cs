using UnityEngine;

public readonly struct GlobalData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly InteractionTargetContext Target;

    private readonly Vector2? _pointerPosition;

    public Vector2? PointerPosition => _pointerPosition;

    public bool IsValid =>
    PointerPosition.HasValue &&
    Target.IsValid;

    public GlobalData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        InteractionTargetContext target = default)
    {
        ItemInstance = itemInstance;
        Target = target;
        _pointerPosition = pointerPosition;
    }
}