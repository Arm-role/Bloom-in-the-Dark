using UnityEngine;

public class Idle_DragState : IDrag
{
    public InteractionInput OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        // Debug.Log(context.PressedActions +" Pressed Action");
        // Debug.Log(context.HeldActions +" Held Action");
        Debug.Log((context.PressedActions != InputActionType.None) +" && "+
                  (context.HeldActions == InputActionType.None));
        
        if (context.ReleasedActions != InputActionType.None)
        {
            var interaction = new InteractionInput(releasedAction: context.ReleasedActions,
                lastPointerPosition: context.CurrentPosition);

            return StateExecutionResult.TransitionWithInteraction(new Release_DragState(), interaction);
        }
        else if (context.PressedActions != InputActionType.None &&
                 context.HeldActions == InputActionType.None)
        {
            return StateExecutionResult.TransitionWithInteraction(
                new Grabbed_DragState(),
                new InteractionInput(
                    pressedAction: context.PressedActions,
                    lastPointerPosition: context.CurrentPosition,
                    stateUpdate: new DragStateUpdate
                    {
                        NewStartPosition = context.CurrentPosition,
                        NewHoldTimer = 0f,
                        NewHasMovedTooMuch = false
                    }));
        }

        return StateExecutionResult.TriggerInteraction(new InteractionInput(
            lastPointerPosition: context.CurrentPosition,
            useSourceItem: context.UseSourceItem));
    }

    public InteractionInput OnExit()
    {
        return null;
    }
}
