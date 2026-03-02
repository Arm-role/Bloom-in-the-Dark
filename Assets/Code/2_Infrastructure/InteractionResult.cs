public readonly struct InteractionResult
{
  public readonly InteractionOutcome Outcome;
  public readonly IWorldCell Cell;
  public readonly WorldAction Action;
  public readonly ETargetType TargetType;
  public readonly ItemCooldownFeedback ItemCooldown;

  public InteractionResult
    (InteractionOutcome outcome, IWorldCell cell, WorldAction action, ETargetType targetType, ItemCooldownFeedback itemCooldownFeedback)
  {
    Outcome = outcome;
    Cell = cell;
    Action = action;
    TargetType = targetType;
    ItemCooldown = itemCooldownFeedback;
  }

  public bool IsConsumed => Outcome == InteractionOutcome.Consumed;
  public bool IsBlocked => Outcome == InteractionOutcome.Blocked;

  public static InteractionResult None
    => new(InteractionOutcome.None, null, null, ETargetType.Default, ItemCooldownFeedback.None);

  public static InteractionResult Blocked(IWorldCell cell, WorldAction action, ETargetType targetType)
    => new(InteractionOutcome.Blocked, cell, action, targetType, ItemCooldownFeedback.None);

  public static InteractionResult Consumed
    (IWorldCell cell, WorldAction action, ETargetType targetType, ItemCooldownFeedback itemCooldown)
    => new(InteractionOutcome.Consumed, cell, action, targetType, itemCooldown);
}