using System.Threading.Tasks;
using UnityEngine;

public class GridTargetingActionPerformer : IActionPerformer
{
    public void Setup() { }

    public bool CanExecute(InteractionHandleContext ctx, IDataProvider data)
    {
        return true;
    }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not GridTargetingData gridData) return Task.FromResult(false);

        var tileState = gridData.TileTarget;

        return tileState.WorldInteractable.TryInteract(context);
    }
}