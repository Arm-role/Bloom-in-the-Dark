public interface IDrag
{
    InteractionInput OnEnter();
    StateExecutionResult OnExecute(DragContext context);
    InteractionInput OnExit();
}
