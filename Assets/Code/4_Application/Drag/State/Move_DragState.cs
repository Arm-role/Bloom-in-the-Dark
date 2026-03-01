using UnityEngine;

public class Move_DragState : IDrag // ลาก
{
    public InteractionInput OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.ReleasedActions != InputActionType.None)
        {
            return StateExecutionResult.TransitionWithInteraction(
                new Release_DragState(),
                new InteractionInput(
                    releasedAction: context.ReleasedActions,  
                    lastPointerPosition: context.CurrentPosition));
        }

        if (context.HeldActions != InputActionType.None)
        {
            return StateExecutionResult.TriggerInteraction(
                new InteractionInput(
                    heldAction: context.HeldActions,
                    lastPointerPosition: context.CurrentPosition));
        }

        return StateExecutionResult.TriggerInteraction(new InteractionInput(lastPointerPosition: context.CurrentPosition));
    }

    public InteractionInput OnExit()
    {
        return null;
    }
}