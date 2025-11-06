using UnityEngine;

public class InteractionResult
{
    public readonly DragStateUpdate StateUpdate;
    public readonly InteractionContext Context;
    public InteractionResult(
        DragStateUpdate stateUpdate = null,
        InputActionType activeAction = InputActionType.None,
        InputActionType releasedAction = InputActionType.None,
        bool useSourceItem = false,
        Vector2? lastPointerPosition = null)
    {
        StateUpdate = stateUpdate;
        Context = new InteractionContext
            (
            activeAction,
            releasedAction,
            useSourceItem,
            lastPointerPosition
            );
    }
}