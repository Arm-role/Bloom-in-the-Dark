using UnityEngine;

public class StateExecutionResult
{
    public readonly IDrag NextState;
    public readonly InteractionInput InteractionInput;

    public StateExecutionResult(IDrag nextState = null, InteractionInput interaction = null)
    {
        NextState = nextState;
        InteractionInput = interaction;
    }

    private static readonly StateExecutionResult _doNothing = new StateExecutionResult();
    public static StateExecutionResult DoNothing() => _doNothing;
    public static StateExecutionResult LastPointerPositionUpdate(Vector2 lastPointerPosition)
    {
        return new StateExecutionResult(interaction: new InteractionInput(lastPointerPosition: lastPointerPosition));
    }
    public static StateExecutionResult TransitionTo(IDrag nextState)
    {
        return new StateExecutionResult(nextState: nextState);
    }
    public static StateExecutionResult TransitionWithLastPointer(IDrag nextState, Vector2 lastPointerPosition)
    {
        return new StateExecutionResult(nextState: nextState, interaction: new InteractionInput(lastPointerPosition: lastPointerPosition));
    }
    public static StateExecutionResult TriggerInteraction(InteractionInput interaction)
    {
        return new StateExecutionResult(interaction: interaction);
    }
    public static StateExecutionResult TransitionWithInteraction(IDrag nextState, InteractionInput interaction)
    {
        return new StateExecutionResult(nextState: nextState, interaction: interaction);
    }
}
