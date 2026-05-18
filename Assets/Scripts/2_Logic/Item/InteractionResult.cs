using System.Threading.Tasks;

public readonly struct InteractionCost
{
  public readonly int ItemCost;

  public InteractionCost(int itemCost) { ItemCost = itemCost; }

  public bool HasItemCost => ItemCost > 0;

  public static readonly InteractionCost None    = new(0);
  public static readonly InteractionCost OneItem = new(1);
}

public readonly struct InteractionResult
{
  public readonly InteractionOutcome Outcome;
  public readonly IWorldCell Cell;
  public readonly WorldAction Action;
  public readonly ETargetType TargetType;
  public readonly ItemCooldownFeedback ItemCooldown;
  public readonly InteractionCost Cost;

  public InteractionResult(
    InteractionOutcome outcome,
    IWorldCell cell,
    WorldAction action,
    ETargetType targetType,
    ItemCooldownFeedback itemCooldownFeedback,
    InteractionCost cost)
  {
    Outcome = outcome;
    Cell = cell;
    Action = action;
    TargetType = targetType;
    ItemCooldown = itemCooldownFeedback;
    Cost = cost;
  }

  public bool IsConsumed => Outcome == InteractionOutcome.Consumed;
  public bool IsBlocked  => Outcome == InteractionOutcome.Blocked;

  public static InteractionResult None
    => new(InteractionOutcome.None, null, null, ETargetType.None, ItemCooldownFeedback.None, InteractionCost.None);

  public static InteractionResult Blocked(IWorldCell cell, WorldAction action, ETargetType targetType)
    => new(InteractionOutcome.Blocked, cell, action, targetType, ItemCooldownFeedback.None, InteractionCost.None);

  // Without declared cost — used by actions that don't consume items.
  public static InteractionResult Consumed(
    IWorldCell cell, WorldAction action, ETargetType targetType, ItemCooldownFeedback itemCooldown)
    => new(InteractionOutcome.Consumed, cell, action, targetType, itemCooldown, InteractionCost.None);

  // With declared cost — action is the authority on what it consumes.
  public static InteractionResult Consumed(
    IWorldCell cell, WorldAction action, ETargetType targetType, ItemCooldownFeedback itemCooldown, InteractionCost cost)
    => new(InteractionOutcome.Consumed, cell, action, targetType, itemCooldown, cost);
}