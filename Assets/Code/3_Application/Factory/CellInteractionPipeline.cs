using System.Threading.Tasks;

public class CellInteractionPipeline
{
  public Task<InteractionResult> Execute(
    InteractionIntent intent,
    WorldCell cell)
  {
    return ExecuteStages(intent, cell);
  }

  public async Task<ETargetType> GetTargetMask(
    InteractionIntent intent,
    WorldCell cell)
  {
    foreach (var stage in InteractionStageOrder.All)
    {
      foreach (var action in cell.ActionRegistry.GetByStage(stage))
      {
        if (!await action.CanProcess(intent, cell))
          continue;

        return action.TargetType;
      }
    }

    return ETargetType.None;
  }

  private async Task<InteractionResult> ExecuteStages(
    InteractionIntent intent,
    WorldCell cell)
  {
    foreach (var stage in InteractionStageOrder.All)
    {
      foreach (var action in cell.ActionRegistry.GetByStage(stage))
      {
        if (!await action.CanProcess(intent, cell))
          continue;

        var result = await action.Process(intent, cell);
        return result;
      }
    }
    return InteractionResult.None;
  }
}