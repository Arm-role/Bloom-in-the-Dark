using System;
using System.Threading.Tasks;
using UnityEngine;

public class DropExecutionResult
{
    public Task<bool> ShouldDestroyTarget { get; set; } = Task.FromResult(false);
    public Action<IItemInstance> SourceItemInstance { get; set; }
    public Action<Collider2D> TargetInteraction { get; set; }
    public string ParticleToPlay { get; set; } = null;
}