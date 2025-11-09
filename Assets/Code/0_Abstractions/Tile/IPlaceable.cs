using UnityEngine.Tilemaps;

public interface IPlaceable
{
    bool CanPlaceOn(TileData targetTile, ETileLayerType targetLayer);
}