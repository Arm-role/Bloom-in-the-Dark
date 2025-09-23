using UnityEngine;

public readonly struct InteractionContext
{
    public readonly bool IsPrimaryAction;
    public readonly bool IsSecondaryAction;
    public readonly bool ShouldClearItem;
    public readonly bool UseSourceItem;
    public readonly Collider2D TargetCollider;
    public readonly Vector2 LastPointerPosition;
    public InteractionContext(
        bool isPrimaryAction = false,
        bool isSecondaryAction = false,
        bool shouldClearItem = false,
        bool useSourceItem = false,
        Collider2D targetCollider = null,
        Vector2 lastPointerPosition = default)
    {
        IsPrimaryAction = isPrimaryAction;
        IsSecondaryAction = isSecondaryAction;
        ShouldClearItem = shouldClearItem;
        UseSourceItem = useSourceItem;
        TargetCollider = targetCollider;
        LastPointerPosition = lastPointerPosition;
    }
}