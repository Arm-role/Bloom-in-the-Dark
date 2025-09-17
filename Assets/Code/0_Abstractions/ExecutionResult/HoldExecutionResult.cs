using System;
using System.Threading.Tasks;

public class HoldExecutionResult
{
    public Task<bool> ShouldDestroySelf { get; set; } = Task.FromResult(false);
    public Action<IItemInstance> SourceInteraction { get; set; }
    public string ParticleToPlay { get; set; } = null;
}
