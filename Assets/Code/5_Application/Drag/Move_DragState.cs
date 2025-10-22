using UnityEngine;

public class Move_DragState : IDrag // ลาก
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsPrimaryActionReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }

        if (context.IsPrimaryAction)
        {
            if (!context.ExceededMoveTolerance &&
                Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
            {
                var update = new DragStateUpdate { NewHasMovedTooMuch = true };
                return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update, lastPointerPosition: context.CurrentPosition));
            }

            if (!context.ExceededMoveTolerance)
            {
                float newTimer = context.ElapsedHoldTime + context.DeltaTime;
                if (newTimer >= context.HoldThresholdTime)
                {
                    InteractionResult primaryActionResult = new InteractionResult(isPrimaryAction: true, lastPointerPosition: context.CurrentPosition);
                    return StateExecutionResult.TransitionWithInteraction(new Hold_DragState(), primaryActionResult);
                }
                else
                {
                    var update = new DragStateUpdate { NewHoldTimer = newTimer };
                    return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update, lastPointerPosition: context.CurrentPosition));
                }
            }
        }

        if (context.IsSecondaryAction)
        {
            if (!context.ExceededMoveTolerance &&
                Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
            {
                var update = new DragStateUpdate { NewHasMovedTooMuch = true };
                return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update, lastPointerPosition: context.CurrentPosition));
            }

            if (!context.ExceededMoveTolerance)
            {
                float newTimer = context.ElapsedHoldTime + context.DeltaTime;
                if (newTimer >= context.HoldThresholdTime)
                {
                    InteractionResult primaryActionResult = new InteractionResult(isSecondaryAction: true, lastPointerPosition: context.CurrentPosition);
                    return StateExecutionResult.TransitionWithInteraction(new Hold_DragState(), primaryActionResult);
                }
                else
                {
                    var update = new DragStateUpdate { NewHoldTimer = newTimer };
                    return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update, lastPointerPosition: context.CurrentPosition));
                }
            }
        }

        return StateExecutionResult.TriggerInteraction(new InteractionResult(lastPointerPosition: context.CurrentPosition));
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}