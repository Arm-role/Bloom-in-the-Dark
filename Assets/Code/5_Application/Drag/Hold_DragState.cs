using UnityEngine;

public class Hold_DragState : IDrag
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }

        if (Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
        {
            return StateExecutionResult.TransitionTo(new Move_DragState());
        }

        return StateExecutionResult.DoNothing();
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
