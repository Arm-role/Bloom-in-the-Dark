using UnityEngine;

public class InteractionResult
{
    public readonly DragStateUpdate StateUpdate;
    public readonly bool IsPrimaryAction;
    public readonly bool IsSecondaryAction;
    public readonly bool ShouldClearItem;
    public readonly IItemInstance SourceItem;
    public readonly Collider2D TargetCollider;
    public InteractionResult(
        DragStateUpdate stateUpdate = null,
        bool isPrimaryAction = false,
        bool isSecondaryAction = false,
        bool shouldClearItem = false,
        IItemInstance sourceItem = null,
        Collider2D targetCollider = null)
    {
        StateUpdate = stateUpdate;
        IsPrimaryAction = isPrimaryAction;
        IsSecondaryAction = isSecondaryAction;
        ShouldClearItem = shouldClearItem;
        SourceItem = sourceItem;
        TargetCollider = targetCollider;
    }
    public static InteractionResult SetItem(IItemInstance itemInstance)
    {
        return new InteractionResult(sourceItem: itemInstance);
    }
}
