using UnityEngine;

public class Release_DragState : IDrag //ปล่อย
{
    public InteractionInput OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        return StateExecutionResult.TransitionWithLastPointer(
            new Idle_DragState(),context.CurrentPosition);
    }

    public InteractionInput OnExit()
    {
        return null;
    }
}
