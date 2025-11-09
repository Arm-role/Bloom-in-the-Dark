public class Idle_DragState : IDrag
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.ReleasedActions != InputActionType.None)
        {
            var interaction = new InteractionResult(releasedAction: context.ReleasedActions,
                lastPointerPosition: context.CurrentPosition);

            return StateExecutionResult.TransitionWithInteraction(new Release_DragState(), interaction);
        }
        else if (context.ActiveActions != InputActionType.None)
        {
            var interaction = new InteractionResult(activeAction: context.ActiveActions, useSourceItem: context.UseSourceItem,
                lastPointerPosition: context.CurrentPosition);

            return StateExecutionResult.TransitionWithInteraction(new Grabbed_DragState(), interaction);
        }

        return StateExecutionResult.TriggerInteraction(new InteractionResult(
            lastPointerPosition: context.CurrentPosition,
            useSourceItem: context.UseSourceItem));
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
