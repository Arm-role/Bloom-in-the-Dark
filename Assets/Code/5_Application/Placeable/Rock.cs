public class Rock : IPlaceable
{
    public bool CanPlaceOn(TileData targetTile, ETileLayerType targetLayer)
    {
        return targetLayer == ETileLayerType.Object &&
               targetTile.baseType != ETileType.Water &&
               !targetTile.HasLayer(ETileLayerType.Object);
    }
}