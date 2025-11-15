using UnityEngine;

public class ProximityInteractionData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly PlacementState? State;

    private readonly Vector2? _pointerPosition;

    public Vector2? PointerPosition => _pointerPosition;

    public bool IsValid =>
    ItemInstance != null &&
    PointerPosition.HasValue &&
    State.HasValue;

    public ProximityInteractionData(
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        PlacementState? state = null)
    {
        ItemInstance = itemInstance;
        State = state;
        _pointerPosition = pointerPosition;
    }
}