using UnityEngine;

public readonly struct GridTargetingData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly TileBaseDataState TileTarget;

    private readonly Vector2? _pointerPosition;
    public Vector2? PointerPosition => _pointerPosition;

    public bool IsValid =>
        ItemInstance != null &&
        PointerPosition.HasValue &&
        TileTarget != null;

    public GridTargetingData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        TileBaseDataState tileTarget = null)
    {
        ItemInstance = itemInstance;
        TileTarget = tileTarget;
        _pointerPosition = pointerPosition;
    }
}
