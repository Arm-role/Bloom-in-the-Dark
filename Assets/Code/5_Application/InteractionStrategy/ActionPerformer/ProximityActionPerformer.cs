using System.Threading.Tasks;
using UnityEngine;

public class ProximityActionPerformer : IActionPerformer
{
    public void Setup() { }

    public bool CanExecute(InteractionHandleContext ctx, IDataProvider data)
    {
        return true;
    }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not ProximityInteractionData proximityData) return Task.FromResult(false);
        return Task.FromResult(false);
    }
}
