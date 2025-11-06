using UnityEngine;

public class Hold_DragState : IDrag
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.ReleasedActions != InputActionType.None)
        {
            return StateExecutionResult.TransitionWithLastPointer(new Release_DragState(), context.CurrentPosition);
        }

        if (Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
        {
            return StateExecutionResult.TransitionWithLastPointer(new Move_DragState(), context.CurrentPosition);
        }

        return StateExecutionResult.LastPointerPositionUpdate(context.CurrentPosition);
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
