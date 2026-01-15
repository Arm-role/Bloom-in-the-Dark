using System.Threading.Tasks;

public class CellInteractionPipeline
{
    private readonly WorldInteractionExecutor _executor;

    public CellInteractionPipeline(WorldInteractionExecutor executor)
    {
        _executor = executor;
    }

    public Task<InteractionResult> Execute(
        InteractionIntent intent,
        WorldCell cell)
    {
        return ExecuteStages(intent, cell);
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

                if (await _executor.Execute(result.Action, cell))
                    return result;
            }
        }

        return InteractionResult.None;
    }
}