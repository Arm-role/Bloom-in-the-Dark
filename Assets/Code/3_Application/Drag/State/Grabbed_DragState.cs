using UnityEngine;

public class Grabbed_DragState : IDrag //คลิก แตะ
{
    public InteractionInput OnEnter()
    {
        var update = new DragStateUpdate { NewHoldTimer = 0f, NewHasMovedTooMuch = false };

        var interaction = new InteractionInput(stateUpdate: update);
        return interaction;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.ReleasedActions != InputActionType.None)
        {
            return StateExecutionResult.TransitionWithInteraction(
                new Release_DragState(),
                new InteractionInput(
                    releasedAction: context.ReleasedActions,  
                    lastPointerPosition: context.CurrentPosition));
        }

        if (context.HeldActions != InputActionType.None)
        {
            float newTimer = context.ElapsedHoldTime + context.DeltaTime;

            if (newTimer >= context.HoldThresholdTime)
            {
                return StateExecutionResult.TransitionWithInteraction(
                    new Hold_DragState(),
                    new InteractionInput(heldAction: context.HeldActions));
            }

            if (Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
            {
                return StateExecutionResult.TransitionWithLastPointer(
                    new Move_DragState(),
                    context.CurrentPosition);
            }

            return StateExecutionResult.TriggerInteraction(
                new InteractionInput(
                    stateUpdate: new DragStateUpdate { NewHoldTimer = newTimer }));
        }


        return StateExecutionResult.LastPointerPositionUpdate(context.CurrentPosition);
    }

    public InteractionInput OnExit()
    {
        return null;
    }
}