public class Grabbed_DragState : IDrag //คลิก แตะ
{
    public InteractionResult OnEnter()
    {
        var update = new DragStateUpdate { NewHoldTimer = 0f, NewHasMovedTooMuch = false };

        var interaction = new InteractionResult(stateUpdate: update);
        return interaction;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.ReleasedActions != InputActionType.None)
        {
            var interaction = new InteractionResult(releasedAction: context.ReleasedActions,
                lastPointerPosition: context.CurrentPosition);

            return StateExecutionResult.TransitionWithInteraction(new Release_DragState(), interaction);
        }

        else if (context.ActiveActions == InputActionType.Primary)
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