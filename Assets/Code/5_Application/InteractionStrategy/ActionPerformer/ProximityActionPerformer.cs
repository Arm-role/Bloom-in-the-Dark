using System.Threading.Tasks;
using UnityEngine;

public class ProximityActionPerformer : IActionPerformer
{
    public void Setup() { }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not ProximityInteractionData proximityData) return Task.FromResult(false);
        return Task.FromResult(false);
    }
}
