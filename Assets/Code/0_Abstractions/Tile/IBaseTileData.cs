using System.Collections.Generic;
using UnityEngine.Tilemaps;

public interface IBaseTileData
{
    public IReadOnlyList<TileBase> Tiles { get; }
    public string DisplayName { get; }
    public ETileLayerType TileLayerType { get; }
    public ETileCapability TileCapability { get; }
}
