using System.Collections.Generic;
using UnityEngine;

public class GameActionFactory
{
  private readonly CellActionContext _ctx;

  public GameActionFactory(CellActionContext ctx)
  {
    _ctx = ctx;
  }

  public IEnumerable<ICellAction> CreateActions(IBaseTileData tile)
  {
    if (tile is SoilTileData)
    {
      yield return new RemoveSoilAction(tile.TargetType);
      yield return new PlantSeedAction(tile.TargetType);
    }

    if (tile is GrassTileData)
      yield return new TillGrassToSoilAction(
        tile.TargetType,
        _ctx.TileLibrary.GetTileBaseDataByName("Soil"));
  }

  public IEnumerable<IGameAction> CreateActions(GameObject ob)
  {
    if (ob.TryGetComponent<PlantGrowthController>(out _))
    {
      yield return new PlantHarvestAction(ETargetType.Interactable);
      yield return new RemoveSeedAction(ETargetType.Interactable);
    }

    if (ob.TryGetComponent<ClearableState>(out _))
      yield return new ClearableAction();

    if (ob.TryGetComponent<OfferingAltarController>(out _))
    {
      yield return new PlaceOfferingAction(ETargetType.Interactable);
      yield return new RemoveOfferingAction(ETargetType.Interactable);
    }

    if (ob.TryGetComponent<BaseBuildingController>(out _))
      yield return new DemolishBuildingAction();
  }
}