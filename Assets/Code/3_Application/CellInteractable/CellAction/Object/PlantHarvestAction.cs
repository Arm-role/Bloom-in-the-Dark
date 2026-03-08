using System.Threading.Tasks;
using UnityEngine;

public class PlantHarvestAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType { get; }

  public PlantHarvestAction(ETargetType targetType)
  {
    TargetType = targetType;
  }

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    var plantState = worldCell.Object.GetComponent<PlantState>();
    return Task.FromResult(
      plantState != null &&
      plantState.IsGrown);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    Debug.Log("Plant Harvest Action");
    
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(cell, result, TargetType);

    var harvest = worldCell.Object.GetComponent<ILootableHandler>();

    ItemStack[] loot;

    if (intent.SourceItem.Data is IToolItemData tool)
      loot = harvest.GetHarvestLoot(tool);
    else
      loot = harvest.GetHarvestLoot();

    foreach (var stack in loot)
      result.ItemRewards.Add(stack);

    result.RemoveObject = true;
    result.RemoveTile = true;
    result.TileTargetLayer = ETileLayerType.Overlay;

    return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None);
  }
}