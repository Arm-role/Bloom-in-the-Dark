public readonly struct InteractionResult
{
    public readonly InteractionOutcome Outcome { get; }
    public readonly WorldAction Source { get; }
    
    public bool IsConsumed => Outcome == InteractionOutcome.Consumed;
    public bool IsBlocked  => Outcome == InteractionOutcome.Blocked;
    
    public InteractionResult(
        InteractionOutcome outcome,
        ICellAction source = null)
    {
        Outcome = outcome;
        Source = source;
    }
    
    public static InteractionResult None
        => new InteractionResult(InteractionOutcome.None);

    public static InteractionResult Consumed(ICellAction source)
        => new InteractionResult(InteractionOutcome.Consumed, source);

    public static InteractionResult Blocked(ICellAction source)
        => new InteractionResult(InteractionOutcome.Blocked, source);
}