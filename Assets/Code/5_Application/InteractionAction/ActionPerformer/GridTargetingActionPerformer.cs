using System.Collections.Generic;
using UnityEngine;

public class GridTargetingActionPerformer : IActionPerformer
{
    private readonly TileLibrary _tileLibrary;
    private readonly TilemapService _tilemapService;

    public GridTargetingActionPerformer(
        TileLibrary tileLibrary,
        TilemapService tilemapService)
    {
        _tileLibrary = tileLibrary;
        _tilemapService = tilemapService;
    }

    public void Setup()
    {
    }

    public void Execute(IDataProvider data)
    {
        if (data is not GridTargetingData gridData)
            return;

        if (gridData.ItemInstance.ItemData is ToolItem toolItem)
        {
            if (toolItem.Name == "Hoe")
            {
                foreach (var info in gridData.PlacementInfos)
                {
                    if (info.State == PlacementState.Valid)
                    {
                        var soilTile = _tileLibrary.GetTileDataByName("Soil");
                        _tilemapService.PlaceTile(info.WorldPosition, soilTile);
                    }
                }
            }
            else if (toolItem.Name == "Pickaxe")
            {
                foreach (var info in gridData.PlacementInfos)
                {
                    if (info.State == PlacementState.Valid)
                    {
                        _tilemapService.RemoveTile(info.WorldPosition);
                    }
                }
            }
        }
    }
}
