using UnityEngine;

public class InteractionResult
{
    public readonly DragStateUpdate StateUpdate;
    public readonly InteractionContext Context;
    public InteractionResult(
        DragStateUpdate stateUpdate = null,
        bool isPrimaryAction = false,
        bool isSecondaryAction = false,
        bool shouldClearItem = false,
        bool useSourceItem = false,
        Collider2D targetCollider = null,
        Vector2 lastPointerPosition = default)
    {
        StateUpdate = stateUpdate;
        Context = new InteractionContext
            (
            isPrimaryAction,
            isSecondaryAction,
            shouldClearItem,
            useSourceItem,
            targetCollider,
            lastPointerPosition
            );
    }
}
