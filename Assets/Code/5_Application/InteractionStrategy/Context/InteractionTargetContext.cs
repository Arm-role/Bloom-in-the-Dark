using UnityEngine;

public readonly struct InteractionTargetContext
{
    public readonly IWorldInteractable WorldInteractable;
    public readonly TileBaseDataState TileState;
    public readonly Vector3 WorldPosition;

    public bool IsTile => TileState != null;
    public bool IsObject => WorldInteractable != null;
    public bool IsValid => WorldInteractable != null || TileState != null;

    public InteractionTargetContext(IWorldInteractable worldInteractable, Vector3 pos)
    {
        WorldInteractable = worldInteractable;
        TileState = null;
        WorldPosition = pos;
    }

    public InteractionTargetContext(TileBaseDataState tileState, Vector3 pos)
    {
        WorldInteractable = null;
        TileState = tileState;
        WorldPosition = pos;
    }

    public IWorldInteractable GetInteractable()
    {
        if (!IsValid) return null;

        if (IsObject)
            return WorldInteractable;

        if (IsTile)
            return TileState.WorldInteractable;

        return null;
    }
}