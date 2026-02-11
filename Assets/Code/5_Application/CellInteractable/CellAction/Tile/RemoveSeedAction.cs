using System.Threading.Tasks;

public class RemoveSeedAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Main;

  public Task<bool> CanProcess(
    InteractionIntent intent,
    IWorldCell cell)
  {
    if (!cell.HasPlacedObject)
      return Task.FromResult(false);

    if (!intent.Is(EInteractionIntentType.Dig))
      return Task.FromResult(false);

    if (!(intent.SourceItem.Data.Name == "Pickaxe"))
      return Task.FromResult(false);

    return Task.FromResult(true);
  }
  
  public Task<InteractionResult> Process(
    InteractionIntent intent,
    IWorldCell cell)
  {
    var result = new WorldAction
    {
      RemoveObject = true,
    };
        
    return Task.FromResult(InteractionResult.Consumed(result));
  }
}