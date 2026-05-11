using System.Threading.Tasks;

public class PlaceUpgradeAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType { get; }

  public PlaceUpgradeAction(ETargetType targetType)
  {
    TargetType = targetType;
  }

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    if (!intent.SourceItem.Data.HasTag(TagLibrary.Get("Item.UsePlace")))
      return Task.FromResult(false);

    return Task.FromResult(true);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(cell, result, TargetType);

    var baseBuilding = worldCell.Object.GetComponent<AltarController>();

    baseBuilding.PlaceItem(intent.SourceItem.Data);

    return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None);
  }
}