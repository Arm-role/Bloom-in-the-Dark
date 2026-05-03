using UnityEngine;

public class Hold_DragState : IDrag
{
    public InteractionInput OnEnter()
    {
        return null;
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

        if (Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
        {
            return StateExecutionResult.TransitionWithLastPointer(new Move_DragState(), context.CurrentPosition);
        }
        else if (context.HeldActions != InputActionType.None)
        {
            return StateExecutionResult.TriggerInteraction(
                new InteractionInput(
                    heldAction: context.HeldActions,
                    lastPointerPosition: context.CurrentPosition));
        }

        return StateExecutionResult.LastPointerPositionUpdate(context.CurrentPosition);
    }

    public InteractionInput OnExit()
    {
        return null;
    }
}
