public class Crop : IPlaceable
{
    public bool CanPlaceOn(TileData targetTile, ETileLayerType targetLayer)
    {
        return targetLayer == ETileLayerType.Crop &&
               targetTile.baseType == ETileType.TilledSoil &&
               !targetTile.HasLayer(ETileLayerType.Crop);
    }
}
