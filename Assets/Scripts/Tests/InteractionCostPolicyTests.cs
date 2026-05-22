#nullable enable
using NUnit.Framework;

// EditMode tests for the pure cost-decision logic extracted from
// InteractionActionRunner. No mocking — every input is a struct or primitive.
public sealed class InteractionCostPolicyTests
{
  // -----------------------------
  // ResolveItemCost
  // -----------------------------

  [TestCase(5, 2, 5)]  // action declared 5 → wins over config 2
  [TestCase(3, 7, 3)]  // action wins even when smaller than config
  [TestCase(0, 4, 4)]  // action declared none → fall back to config
  [TestCase(0, 0, 0)]  // both zero
  public void ResolveItemCost_ActionCostOverridesConfig(
    int actionCost, int configCost, int expected)
  {
    var result = MakeResult(actionCost);
    var feedback = MakeFeedback(itemCost: configCost);

    Assert.AreEqual(expected, InteractionCostPolicy.ResolveItemCost(result, feedback));
  }

  // -----------------------------
  // ShouldConsumeItem
  // -----------------------------

  [TestCase(2, true,  false, true)]   // has cost, has item, no rewards → consume
  [TestCase(0, true,  false, false)]  // cost zero → skip
  [TestCase(2, false, false, false)]  // no source item → skip
  [TestCase(2, true,  true,  false)]  // action gave rewards → skip (harvest case)
  public void ShouldConsumeItem_RespectsRewardOverride(
    int itemCost, bool hasItem, bool actionGaveRewards, bool expected)
  {
    Assert.AreEqual(expected,
      InteractionCostPolicy.ShouldConsumeItem(itemCost, hasItem, actionGaveRewards));
  }

  // -----------------------------
  // LacksRequiredItem
  // -----------------------------

  [TestCase(2, false, true)]   // requires item + none in hand → lacks
  [TestCase(2, true,  false)]  // requires item + has item → ok
  [TestCase(0, false, false)]  // no requirement → ok even without item
  public void LacksRequiredItem_OnlyWhenCostSetAndNoItem(
    int itemCost, bool hasItem, bool expected)
  {
    var feedback = MakeFeedback(itemCost: itemCost);

    Assert.AreEqual(expected, InteractionCostPolicy.LacksRequiredItem(feedback, hasItem));
  }

  // --- helpers ---

  private static InteractionResult MakeResult(int actionCost)
    => new InteractionResult(
         InteractionOutcome.Consumed,
         cell: null,
         action: null,
         ETargetType.None,
         ItemCooldownFeedback.None,
         new InteractionCost(actionCost));

  private static InteractionFeedback MakeFeedback(int itemCost)
    => new InteractionFeedback(default, energyCost: 0, itemCost: itemCost, playerCooldown: 0f);
}
