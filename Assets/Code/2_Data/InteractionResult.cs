public readonly struct InteractionResult
{
    public readonly InteractionOutcome Outcome;
    public readonly WorldAction Action;

    public InteractionResult(InteractionOutcome outcome, WorldAction action)
    {
        Outcome = outcome;
        Action = action;
    }

    public bool IsConsumed => Outcome == InteractionOutcome.Consumed;
    public bool IsBlocked => Outcome == InteractionOutcome.Blocked;

    public static InteractionResult None
        => new(InteractionOutcome.None, null);

    public static InteractionResult Blocked(WorldAction action)
        => new(InteractionOutcome.Blocked, action);

    public static InteractionResult Consumed(WorldAction action)
        => new(InteractionOutcome.Consumed, action);
}