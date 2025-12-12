using System;
using System.Threading.Tasks;

public class ActionExecutionResult
{
    public Action<AuxiliaryInput> ModifierInput { get; set; }
    public Action<ITargetDetector> TargetDetector { get; set; }
    public Action<ITargetValidator> TargetValidator { get; set; }
    public Action<IInventoryLogic> InventoryInteraction { get; set; }
    public Action<IPlayerEnergy> PlayerEnergy { get; set; }
    public Action<IPlayerState> PlayerState { get; set; }
    public Action<IActionPerformer> ActionPerformer { get; set; }

    public string ParticleToPlay { get; set; } = null;
    public Task<bool> ShouldSpawnSelf { get; set; } = Task.FromResult(false);
}
