using System.Collections.Generic;
using UnityEngine.Tilemaps;

public readonly struct GridTargetingData : IDataProvider
{
    public readonly IItemInstance ItemInstance;
    public readonly List<TileInfo> PlacementInfos;
    public GridTargetingData(
       IItemInstance itemInstance = null,
        List<TileInfo> placementInfos = null,
        TileBase tileBase = null)
    {
        ItemInstance = itemInstance;
        PlacementInfos = placementInfos;
    }
}