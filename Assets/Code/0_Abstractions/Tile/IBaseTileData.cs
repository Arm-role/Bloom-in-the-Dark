using System.Collections.Generic;
using UnityEngine.Tilemaps;

public interface IBaseTileData
{
    public TileBase Tile { get; }
    public string DisplayName { get; }
    public ETileLayerType TileLayerType { get; }
    public ETileType TileType { get; }
    public EWorldInteractableType WorldInteractableType { get; }
}