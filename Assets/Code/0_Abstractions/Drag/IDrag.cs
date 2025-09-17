public interface IDrag
{
    InteractionResult OnEnter();
    StateExecutionResult OnExecute(DragContext context);
    InteractionResult OnExit();
}
