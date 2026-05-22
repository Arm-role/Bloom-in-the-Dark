#nullable enable

// Pure decision rules for item cost during an interaction.
// No dependencies — all inputs are passed in as structs/primitives so the
// policy is fully unit-testable without mocking.
public static class InteractionCostPolicy
{
  // Action-declared cost takes priority; config cost is the fallback for
  // actions that don't carry their own (e.g. skill attacks).
  public static int ResolveItemCost(
    InteractionResult result,
    InteractionFeedback feedback)
    => result.Cost.HasItemCost ? result.Cost.ItemCost : feedback.ItemCost;

  // Don't consume an item if the action itself granted item rewards
  // (e.g. harvest — the player gets the produce, no input item is "spent").
  public static bool ShouldConsumeItem(
    int itemCost,
    bool hasSourceItem,
    bool actionGaveRewards)
    => itemCost > 0 && hasSourceItem && !actionGaveRewards;

  // Cost says the action needs an item but the player isn't holding one.
  public static bool LacksRequiredItem(
    InteractionFeedback feedback,
    bool hasSourceItem)
    => feedback.ItemCost > 0 && !hasSourceItem;
}
