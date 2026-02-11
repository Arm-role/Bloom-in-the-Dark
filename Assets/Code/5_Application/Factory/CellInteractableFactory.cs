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
    if (tile is SoilTileData)
    {
      yield return new RemoveSoilAction();
      yield return new PlantSeedAction();
    }

    if (tile is GrassTileData)
      yield return new TillGrassToSoilAction(
        _ctx.TileLibrary.GetTileBaseDataByName("Soil"));
  }

  public IEnumerable<ICellAction> CreateActions(GameObject ob)
  {
    if (ob.TryGetComponent<PlantGrowthController>(out _))
    {
      yield return new PlantHarvestAction();
      yield return new RemoveSeedAction();
    }

    if (ob.TryGetComponent<ClearableState>(out _))
      yield return new ClearableAction();
  }
}