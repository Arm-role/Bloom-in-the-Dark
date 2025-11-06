using System;
using System.Threading.Tasks;
using UnityEngine;

public class DropExecutionResult
{
    public Action<IItemInstance> SourceItemInstance { get; set; }
    public Action<InteractionTarget> TargetInteraction { get; set; }
    public string ParticleToPlay { get; set; } = null;
}