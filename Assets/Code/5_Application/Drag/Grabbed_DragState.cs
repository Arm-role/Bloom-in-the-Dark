public class Grabbed_DragState : IDrag //คลิก แตะ
{
    public InteractionResult OnEnter()
    {
        var update = new DragStateUpdate { NewHoldTimer = 0f, NewHasMovedTooMuch = false };
        // และอาจจะสั่งให้จัดลำดับการแสดงผลของไอเทม
        var interaction = new InteractionResult(stateUpdate: update);
        return interaction;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsPrimaryAction)
        {
            return StateExecutionResult.TransitionTo(new Move_DragState());
        }
        else if (context.IsReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }

        return StateExecutionResult.DoNothing();
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}
