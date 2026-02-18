public readonly struct InteractionResult
{
  public readonly InteractionOutcome Outcome;
  public readonly IWorldCell Cell;
  public readonly WorldAction Action;
  public readonly ETargetType TargetType;

  public InteractionResult(InteractionOutcome outcome, IWorldCell cell, WorldAction action, ETargetType targetType)
  {
    Outcome = outcome;
    Cell = cell;
    Action = action;
    TargetType = targetType;
  }

  public bool IsConsumed => Outcome == InteractionOutcome.Consumed;
  public bool IsBlocked => Outcome == InteractionOutcome.Blocked;

  public static InteractionResult None
    => new(InteractionOutcome.None, null, null, ETargetType.Default);

  public static InteractionResult Blocked(IWorldCell cell, WorldAction action, ETargetType targetType)
    => new(InteractionOutcome.Blocked, cell, action, targetType);

  public static InteractionResult Consumed(IWorldCell cell, WorldAction action, ETargetType targetType)
    => new(InteractionOutcome.Consumed, cell, action, targetType);
}