
using UnityEngine;

public class Grabbed_DragState : IDrag //คลิก แตะ
{
    public InteractionResult OnEnter()
    {
        var update = new DragStateUpdate { NewHoldTimer = 0f, NewHasMovedTooMuch = false };
        // และอาจจะสั่งให้จัดลำดับการแสดงผลของไอเทม
        var interaction = new InteractionResult(stateUpdate: update);
        return interaction;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsPrimaryAction)
        {
            return StateExecutionResult.TransitionWithLastPointer(new Move_DragState(), context.CurrentPosition);
        }
        else if (context.IsPrimaryActionReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }

        if (context.IsSecondaryAction)
        {
            return StateExecutionResult.TransitionWithLastPointer(new Move_DragState(), context.CurrentPosition);
        }
        else if (context.IsSecondaryActionReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }
        return StateExecutionResult.LastPointerPositionUpdate(context.CurrentPosition);
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
