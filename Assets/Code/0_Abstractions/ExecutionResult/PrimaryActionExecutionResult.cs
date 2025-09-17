using System;
using System.Threading.Tasks;

public class PrimaryActionExecutionResult
{
    public Task<bool> ShouldSpawnSelf { get; set; } = Task.FromResult(false);
    public Action<IItemInstance> SourceItemInstance { get; set; }
    public string ParticleToPlay { get; set; } = null;
}
