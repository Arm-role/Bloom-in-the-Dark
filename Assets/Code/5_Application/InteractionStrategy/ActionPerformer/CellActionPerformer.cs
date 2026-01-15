using System.Threading.Tasks;
using UnityEngine;

public class CellActionPerformer : IActionPerformer
{
    private readonly CellInteractionPipeline _pipeline;

    public CellActionPerformer(CellInteractionPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public bool CanExecute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        return target.IsValid &&
               target.Cells != null &&
               target.Cells.Count > 0;
    }

    public async Task<InteractionResult> Execute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        Debug.Log("CellActionPerformer");
        InteractionResult finalResult = InteractionResult.None;
        var intent = ctx.ToIntent();

        foreach (var cell in target.Cells)
        {
            finalResult = await _pipeline.Execute(intent, cell);

            if (finalResult.IsConsumed)
                break;
        }

        return finalResult;
    }
}