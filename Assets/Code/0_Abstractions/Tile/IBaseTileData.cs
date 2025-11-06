using System.Collections.Generic;
using UnityEngine.Tilemaps;

public interface IBaseTileData
{
    public TileBase Tile { get; }
    public string DisplayName { get; }
    public bool CanBeReplaced { get; }
    public IReadOnlyList<string> ReplaceableTiles { get; }
}