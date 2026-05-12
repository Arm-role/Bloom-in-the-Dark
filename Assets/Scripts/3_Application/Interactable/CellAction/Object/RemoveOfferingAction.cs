using System.Threading.Tasks;

public class RemoveOfferingAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType { get; }

  public RemoveOfferingAction(ETargetType targetType)
  {
    TargetType = targetType;
  }

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if ((intent.Input & InputActionType.Secondary) == 0)
      return Task.FromResult(false);

    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    var offering = worldCell.Object.GetComponent<OfferingAltarController>();
    return Task.FromResult(offering != null && offering.IsOccupied);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(cell, result, TargetType);

    var offering = worldCell.Object.GetComponent<OfferingAltarController>();
    var removed = offering.RemoveItem();

    if (removed == null)
      return InteractionResult.Blocked(cell, result, TargetType);

    result.ItemRewards.Add(new ItemStack(removed.Data, 1, removed));

    return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None);
  }
}
