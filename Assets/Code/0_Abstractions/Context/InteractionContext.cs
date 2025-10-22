using UnityEngine;

public readonly struct InteractionContext
{
    public readonly bool IsPrimaryAction;
    public readonly bool IsSecondaryAction;
    public readonly bool UseSourceItem;
    public readonly Collider2D TargetCollider;
    public readonly Vector2? LastPointerPosition;
    public InteractionContext(
        bool isPrimaryAction = false,
        bool isSecondaryAction = false,
        bool useSourceItem = false,
        Collider2D targetCollider = null,
        Vector2? lastPointerPosition = null)
    {
        IsPrimaryAction = isPrimaryAction;
        IsSecondaryAction = isSecondaryAction;
        UseSourceItem = useSourceItem;
        TargetCollider = targetCollider;
        LastPointerPosition = lastPointerPosition;
    }
}
