using UnityEngine;

public class ProximityInteractionData : IDataProvider
{
    public readonly Vector2? PlayerPosition;
    public readonly IItemInstance ItemInstance;
    public readonly Vector2? PointerPosition;
    public readonly Vector2? Direction;
    public readonly PlacementState? State;

    public ProximityInteractionData(
        Vector2? playerPosition = null,
        IItemInstance itemInstance = null,
        Vector2? pointerPosition = null,
        Vector2? direction = null,
        PlacementState? state = null)
    {
        PlayerPosition = playerPosition;
        ItemInstance = itemInstance;
        PointerPosition = pointerPosition;
        Direction = direction;
        State = state;
    }
}