using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GridBaseData : IDataProvider
{
    public readonly Transform Player;
    public readonly List<TileInfo> PlacementInfos;

    public GridBaseData(
        Transform player = null, 
        List<TileInfo> placementInfos = null)
    {
        Player = player;
        PlacementInfos = placementInfos;
    }
}
