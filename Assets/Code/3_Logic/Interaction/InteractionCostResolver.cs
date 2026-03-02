public class InteractionCostResolver
{
  private readonly InteractionCostConfig _config;
  private readonly InteractionRuntimeState _runtime;

  public InteractionCostResolver(
    InteractionCostConfig config,
    InteractionRuntimeState runtime)
  {
    _config = config;
    _runtime = runtime;
  }

  public bool TryResolve(
    EInteractionIntentType type,
    ItemCategoryData  itemData,
    ETargetType target,
    out InteractionFeedback feedback)
  {
    feedback = InteractionFeedback.None(type);

    // --- Intent Cost ---
    if (!_config.TryGetIntentCost(type, itemData, target, out var baseCost))
      return false;

    int energy = baseCost.EnergyCost;
    int itemCost = baseCost.ItemCost;
    float cooldown = baseCost.Cooldown;

    feedback = new InteractionFeedback(
      type,
      energy,
      itemCost,
      cooldown
    );

    return true;
  }

  public void ApplyCost(
    InteractionIntent intent,
    InteractionFeedback cost)
  {
    // Apply cooldown
    _runtime.SetCooldown("GLOBAL", _config.GlobalCooldown);
    _runtime.SetCooldown($"INTENT_{intent.Type}", cost.PlayerCooldown);

    if (intent.SourceItem != null)
    {
      string itemKey = $"ITEM_{intent.SourceItem.Data.ID}";
      _runtime.SetCooldown(itemKey, cost.PlayerCooldown);
    }
  }
}