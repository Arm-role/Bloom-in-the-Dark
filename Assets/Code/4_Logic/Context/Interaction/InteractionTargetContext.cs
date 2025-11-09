using UnityEngine;

public readonly struct InteractionTargetContext
{
    public readonly Collider2D Collider;
    public readonly TileBaseDataState TileState;
    public readonly Vector3 WorldPosition;

    public bool IsTile => TileState != null;
    public bool IsObject => Collider != null;
    public bool IsValid => Collider != null || TileState != null;

    public InteractionTargetContext(Collider2D collider, Vector3 pos)
    {
        Collider = collider;
        TileState = null;
        WorldPosition = pos;
    }

    public InteractionTargetContext(TileBaseDataState tileState, Vector3 pos)
    {
        Collider = null;
        TileState = tileState;
        WorldPosition = pos;
    }

}