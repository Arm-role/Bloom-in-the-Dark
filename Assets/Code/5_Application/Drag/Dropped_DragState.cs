using System.Linq;
using UnityEngine;

public class Dropped_DragState : IDrag 
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        var target = context.HitColliders.FirstOrDefault();

        if (target != null)
        {
            InteractionResult dropResult = new InteractionResult(targetCollider: target, lastPointerPosition: context.CurrentPosition);
            return StateExecutionResult.TransitionWithInteraction(new Idle_DragState(), dropResult);
        }

        return StateExecutionResult.TransitionWithLastPointer(new Idle_DragState(), context.CurrentPosition);
    }
    public InteractionResult OnExit()
    {
        return null;
    }
}
