using UnityEngine;

public readonly struct DragContext
{
    public readonly Collider2D[] HitColliders;

    public readonly bool UseSourceItem;

    public readonly Vector2 StartPosition;
    public readonly Vector2 CurrentPosition;
    
    public readonly float MoveTolerance;
    public readonly float HoldThresholdTime;
    public readonly float DeltaTime;

    public readonly float ElapsedHoldTime;
    public readonly bool ExceededMoveTolerance;

    public readonly bool IsPrimaryAction;
    public readonly bool IsSecondaryAction;
    public readonly bool IsReleased;

    public DragContext(
     Collider2D[] hitColliders,
     bool useSourceItem,
     Vector2 startPosition,
     Vector2 currentPosition,
     float moveTolerance,
     float holdThresholdTime,
     float deltaTime,
     float elapsedHoldTime,
     bool exceededMoveTolerance,
     bool isPrimaryAction,
     bool isSecondaryAction,
     bool isReleased)
    {
        HitColliders = hitColliders;

        UseSourceItem = useSourceItem;

        StartPosition = startPosition;
        CurrentPosition = currentPosition;
        MoveTolerance = moveTolerance;
        HoldThresholdTime = holdThresholdTime;
        DeltaTime = deltaTime;

        ElapsedHoldTime = elapsedHoldTime;
        ExceededMoveTolerance = exceededMoveTolerance;

        IsPrimaryAction = isPrimaryAction;
        IsSecondaryAction = isSecondaryAction;
        IsReleased = isReleased;
    }
}
