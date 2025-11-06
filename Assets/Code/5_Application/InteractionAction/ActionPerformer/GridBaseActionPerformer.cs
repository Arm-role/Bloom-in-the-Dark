using System.Collections.Generic;
using UnityEngine;

public class GridBaseActionPerformer : IActionPerformer
{
    private readonly TileLibrary _tileLibrary;
    private readonly TilemapService _tilemapService;


    public GridBaseActionPerformer(TileLibrary tileLibrary, TilemapService tilemapService)
    {
        _tileLibrary = tileLibrary;
        _tilemapService = tilemapService;
    }

    public void Setup()
    {
    }

    public void Execute(IDataProvider data)
    {
        if (data is not GridBaseData gridData) return;

        foreach (var placePoint in gridData.PlacementInfos)
        {
            var soilTile = _tileLibrary.GetTileDataByName("Soil");
            _tilemapService.PlaceTile(placePoint.WorldPosition, soilTile);
        }
    }
}
