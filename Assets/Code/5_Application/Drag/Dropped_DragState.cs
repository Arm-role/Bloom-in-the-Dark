using System.Linq;
using UnityEngine;

public class Dropped_DragState : IDrag // ตกใส่เป้าหมาย
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

        return StateExecutionResult.TransitionTo(new Idle_DragState());
    }
    public InteractionResult OnExit()
    {
        return null;
    }
}
