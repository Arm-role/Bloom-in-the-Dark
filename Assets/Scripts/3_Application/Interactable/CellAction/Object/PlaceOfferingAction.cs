using System.Threading.Tasks;

public class PlaceOfferingAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType { get; }

  public PlaceOfferingAction(ETargetType targetType)
  {
    TargetType = targetType;
  }

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if ((intent.Input & InputActionType.Secondary) == 0)
      return Task.FromResult(false);

    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    if (!intent.HasItem)
      return Task.FromResult(false);

    var offering = worldCell.Object.GetComponent<OfferingAltarController>();
    if (offering == null || offering.IsOccupied)
      return Task.FromResult(false);

    return Task.FromResult(true);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(cell, result, TargetType);

    var offering = worldCell.Object.GetComponent<OfferingAltarController>();
    if (!offering.TryPlaceItem(intent.SourceItem))
      return InteractionResult.Blocked(cell, result, TargetType);

    return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None, InteractionCost.OneItem);
  }
}
