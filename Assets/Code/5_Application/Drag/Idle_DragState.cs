using UnityEngine;

public class Idle_DragState : IDrag
{
    public InteractionResult OnEnter()
    {
        return null;
    }

    public StateExecutionResult OnExecute(DragContext context)
    {
        if (context.IsPrimaryAction)
        {
            var interaction = new InteractionResult(isPrimaryAction: context.IsPrimaryAction, useSourceItem: context.UseSourceItem,
                lastPointerPosition: context.CurrentPosition);

            return StateExecutionResult.TransitionWithInteraction(new Grabbed_DragState(), interaction);
        }

        if (context.IsSecondaryAction)
        {
            var interaction = new InteractionResult(isPrimaryAction: context.IsSecondaryAction, useSourceItem: context.UseSourceItem,
                lastPointerPosition: context.CurrentPosition);
            return StateExecutionResult.TransitionWithInteraction(new Grabbed_DragState(), interaction);
        }
        return StateExecutionResult.TriggerInteraction(new InteractionResult(lastPointerPosition : context.CurrentPosition, useSourceItem: context.UseSourceItem));
    }

    public InteractionResult OnExit()
    {
        return null;
    }
}