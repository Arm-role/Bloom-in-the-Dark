using System.Collections.Generic;
using UnityEngine;

public class CellInteractableFactory
{
    private readonly CellActionContext _ctx;

    public CellInteractableFactory(CellActionContext ctx)
    {
        _ctx = ctx;
    }

    public IEnumerable<ICellAction> CreateActions(IBaseTileData tile)
    {
        if (tile is SoilTileData soilTileData)
        {
            yield return new RemoveSoilAction();
            yield return new PlantSeedAction();
        }

        if (tile is GrassTileData gasTileData)
            yield return new TillGrassToSoilAction(
                _ctx.TileLibrary.GetTileBaseDataByName("Soil"));
    }
    public IEnumerable<ICellAction> CreateActions(GameObject ob)
    {
        if (ob.TryGetComponent<PlantState>(out var state))
            yield return new PlantHarvestAction();
        
    }
}