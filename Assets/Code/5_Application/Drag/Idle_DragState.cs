public class Idle_DragState : IDrag
{
    public InteractionResult OnEnter()
    {
        return new InteractionResult(shouldClearItem: true);
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsPrimaryAction)
        {
            var interaction = new InteractionResult(isPrimaryAction: context.IsPrimaryAction, sourceItem: context.SourceItem);
            return StateExecutionResult.TransitionWithInteraction(new Grabbed_DragState(), interaction);
        }
        return StateExecutionResult.DoNothing();
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}