using UnityEngine;

public class InteractionInput
{
    public readonly DragStateUpdate StateUpdate;
    public readonly InteractionContext Context;
    public InteractionInput(
        DragStateUpdate stateUpdate = null,
        InputActionType pressedAction = InputActionType.None,
        InputActionType heldAction = InputActionType.None,
        InputActionType releasedAction = InputActionType.None,
        bool useSourceItem = false,
        Vector2? lastPointerPosition = null)
    {
        StateUpdate = stateUpdate;
        Context = new InteractionContext
            (
            pressedAction,
            heldAction,
            releasedAction,
            useSourceItem,
            lastPointerPosition
            );
    }
}

