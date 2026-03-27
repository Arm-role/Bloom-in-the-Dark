using System.Threading.Tasks;

public class CellInteractionPipeline
{
  public async Task<InteractionResult> Execute(
  InteractionIntent intent,
  WorldCell cell)
  {
    var action = await Resolve(intent, cell);

    if (action == null)
      return InteractionResult.None;

    if (action is not ICellAction cellAction)
      return InteractionResult.None;

    return await cellAction.Process(intent, cell);
  }


  public async Task<ETargetType> GetTargetMask(
  InteractionIntent intent,
  WorldCell cell)
  {
    var action = await Resolve(intent, cell);

    return action?.TargetType ?? ETargetType.None;
  }


  public async Task<IGameAction> Resolve(
  InteractionIntent intent,
  WorldCell cell)
  {
    foreach (var stage in InteractionStageOrder.All)
    {
      foreach (var action in cell.ActionRegistry.GetByStage(stage))
      {
        if (action is not ICellAction cellAction)
          continue;

        if (!await cellAction.CanProcess(intent, cell))
          continue;

        return action;
      }
    }

    return null;
  }
}
