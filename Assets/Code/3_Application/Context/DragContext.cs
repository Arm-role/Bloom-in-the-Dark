using UnityEngine;

public readonly struct DragContext
{
    public readonly bool UseSourceItem;

    public readonly Vector2 StartPosition;
    public readonly Vector2 CurrentPosition;

    public readonly float MoveTolerance;
    public readonly float HoldThresholdTime;
    public readonly float DeltaTime;

    public readonly float ElapsedHoldTime;
    public readonly bool ExceededMoveTolerance;

    public readonly InputActionType PressedActions;
    public readonly InputActionType HeldActions;
    public readonly InputActionType ReleasedActions;

    public DragContext(
     bool useSourceItem,
     Vector2 startPosition,
     Vector2 currentPosition,
     float moveTolerance,
     float holdThresholdTime,
     float deltaTime,
     float elapsedHoldTime,
     bool exceededMoveTolerance,
     InputActionType pressedActions,
     InputActionType heldActions,
     InputActionType releasedActions)
    {

        UseSourceItem = useSourceItem;

        StartPosition = startPosition;
        CurrentPosition = currentPosition;
        MoveTolerance = moveTolerance;
        HoldThresholdTime = holdThresholdTime;
        DeltaTime = deltaTime;

        ElapsedHoldTime = elapsedHoldTime;
        ExceededMoveTolerance = exceededMoveTolerance;

        PressedActions = pressedActions;
        HeldActions = heldActions;
        ReleasedActions = releasedActions;
    }
}
