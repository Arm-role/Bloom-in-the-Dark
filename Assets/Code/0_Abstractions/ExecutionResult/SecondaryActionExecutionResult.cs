using System;
using System.Threading.Tasks;

public class SecondaryActionExecutionResult
{
    public Task<bool> ShouldDestroyTarget { get; set; } = Task.FromResult(false);
    public Action<IItemInstance> SourceInteraction { get; set; }
    public string ParticleToPlay { get; set; } = null;
}