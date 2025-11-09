using System.Collections.Generic;
using UnityEngine;

public class GridBaseActionPerformer : IActionPerformer
{
    private readonly TileLibrary _tileLibrary;
    private readonly WorldTileManager _worldTileManager;


    public GridBaseActionPerformer(TileLibrary tileLibrary, WorldTileManager worldTilemanager)
    {
        _tileLibrary = tileLibrary;
        _worldTileManager = worldTilemanager;
    }

    public void Setup()
    {
    }

    public void Execute(IDataProvider data)
    {
        if (data is not GridBaseData gridData) return;

        foreach (var placePoint in gridData.PlacementInfos)
        {
            //var soilTile = _tileLibrary.GetTileDataByName("Soil");
            //_tilemapService.PlaceTile(placePoint.WorldPosition, soilTile);
        }
    }
}
