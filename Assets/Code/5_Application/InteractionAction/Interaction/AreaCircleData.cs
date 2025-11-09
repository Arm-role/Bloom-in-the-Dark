using UnityEngine;

public readonly struct AreaCircleData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PointerPosition;
    public readonly PlacementState? State;

    public AreaCircleData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null, 
        PlacementState? state = null)
    {
        ItemInstance = itemInstance;
        PointerPosition = pointerPosition;
        State = state;
    }
}
