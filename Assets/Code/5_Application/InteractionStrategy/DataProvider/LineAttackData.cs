using UnityEngine;

public class LineAttackData : IDataProvider
{
    public IItemInstance ItemInstance;

    public Vector2? Origin;
    public Vector2? Direction;
    public Vector2? PointerPosition => null;

    public bool IsValid =>
    Direction.HasValue;

    public LineAttackData(
      IItemInstance itemInstance = null,
      Vector2? origin = null,
      Vector2 direction = default)
    {
        ItemInstance = itemInstance;
        Origin = origin;
        Direction = direction;
    }
}