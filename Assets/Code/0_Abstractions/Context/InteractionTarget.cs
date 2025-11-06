using UnityEngine;

public readonly struct InteractionTarget
{
    public readonly Collider2D Collider;    
    public readonly IBaseTileData TileData;  
    public readonly Vector3 WorldPosition; 

    public bool IsTile => TileData != null;
    public bool IsObject => Collider != null;
    public bool IsVaild => TileData != null || Collider;

    public InteractionTarget(Collider2D collider, Vector3 pos)
    {
        Collider = collider;
        TileData = null;
        WorldPosition = pos;
    }

    public InteractionTarget(IBaseTileData tileData, Vector3 pos)
    {
        Collider = null;
        TileData = tileData;
        WorldPosition = pos;
    }
}