using UnityEngine;

public class Release_DragState : IDrag //ปล่อย
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        //if (context.Target.IsVaild)
        //{
        //    return StateExecutionResult.TransitionWithLastPointer(new Dropped_DragState(), context.CurrentPosition);
        //}

        return StateExecutionResult.TransitionWithLastPointer(new Idle_DragState(), context.CurrentPosition);
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
