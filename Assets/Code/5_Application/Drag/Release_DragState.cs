using UnityEngine;

public class Release_DragState : IDrag //ปล่อย
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        bool foundOther = context.HitColliders.Length > 0;

        if (foundOther)
        {
            return StateExecutionResult.TransitionTo(new Dropped_DragState());
        }

        return StateExecutionResult.TransitionTo(new Idle_DragState());
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
