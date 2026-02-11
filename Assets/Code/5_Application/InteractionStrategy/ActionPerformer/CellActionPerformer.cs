using System.Threading.Tasks;

public class CellActionPerformer : IActionPerformer
{
    private readonly CellInteractionPipeline _pipeline;

    public CellActionPerformer(CellInteractionPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public bool CanExecute(
        InteractionIntent intent,
        TargetResult target)
    {
        return target.IsValid &&
               target.Cells != null &&
               target.Cells.Count > 0;
    }

    public async Task<InteractionResult> Execute(
        InteractionIntent intent,
        TargetResult target)
    {
        InteractionResult finalResult = InteractionResult.None;

        foreach (var cell in target.Cells)
        {
            finalResult = await _pipeline.Execute(intent, cell);

            if (finalResult.IsConsumed)
                break;
        }

        return finalResult;
    }
}