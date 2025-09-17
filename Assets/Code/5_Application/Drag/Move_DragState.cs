using UnityEngine;

public class Move_DragState : IDrag // ลาก
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsReleased)
        {
            return StateExecutionResult.TransitionTo(new Release_DragState());
        }

        if (context.IsPrimaryAction)
        {
            if (!context.ExceededMoveTolerance &&
                Vector2.Distance(context.CurrentPosition, context.StartPosition) > context.MoveTolerance)
            {
                var update = new DragStateUpdate { NewHasMovedTooMuch = true };
                return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update));
            }

            if (!context.ExceededMoveTolerance)
            {
                float newTimer = context.ElapsedHoldTime + context.DeltaTime;
                if (newTimer >= context.HoldThresholdTime)
                {
                    InteractionResult primaryActionResult = new InteractionResult(isPrimaryAction: true);
                    return StateExecutionResult.TransitionWithInteraction(new Hold_DragState(), primaryActionResult);
                }
                else
                {
                    var update = new DragStateUpdate { NewHoldTimer = newTimer };
                    return StateExecutionResult.TriggerInteraction(new InteractionResult(stateUpdate: update));
                }
            }
        }

        return StateExecutionResult.DoNothing();
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}