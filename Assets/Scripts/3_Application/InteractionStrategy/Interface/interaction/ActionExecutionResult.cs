public struct ActionExecutionResult
{
    public InteractionResult WorldResult;
    public InteractionFeedback Feedback;

    public bool IsConsumed =>
        WorldResult.IsConsumed;

    public bool IsBlocked =>
        WorldResult.IsBlocked;
}