using UnityEngine;

public class StateExecutionResult
{
    public readonly IDrag NextState;
    public readonly InteractionResult InteractionResult;

    public StateExecutionResult(IDrag nextState = null, InteractionResult interaction = null)
    {
        NextState = nextState;
        InteractionResult = interaction;
    }

    private static readonly StateExecutionResult _doNothing = new StateExecutionResult();
    public static StateExecutionResult DoNothing() => _doNothing;
    public static StateExecutionResult LastPointerPositionUpdate(Vector2 lastPointerPosition)
    {
        return new StateExecutionResult(interaction: new InteractionResult(lastPointerPosition: lastPointerPosition));
    }
    public static StateExecutionResult TransitionTo(IDrag nextState)
    {
        return new StateExecutionResult(nextState: nextState);
    }
    public static StateExecutionResult TransitionWithLastPointer(IDrag nextState, Vector2 lastPointerPosition)
    {
        return new StateExecutionResult(nextState: nextState, interaction: new InteractionResult(lastPointerPosition: lastPointerPosition));
    }
    public static StateExecutionResult TriggerInteraction(InteractionResult interaction)
    {
        return new StateExecutionResult(interaction: interaction);
    }
    public static StateExecutionResult TransitionWithInteraction(IDrag nextState, InteractionResult interaction)
    {
        return new StateExecutionResult(nextState: nextState, interaction: interaction);
    }
}
