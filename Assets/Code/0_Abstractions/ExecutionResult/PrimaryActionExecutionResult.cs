using System;
using System.Threading.Tasks;

public class PrimaryActionExecutionResult
{
    public Action<IInteractionHandle> InteractionHandle { get; set; }
    public Action<IInventoryLogic> InventoryInteraction { get; set; }
    public string ParticleToPlay { get; set; } = null;
    public Task<bool> ShouldSpawnSelf { get; set; } = Task.FromResult(false);
}
