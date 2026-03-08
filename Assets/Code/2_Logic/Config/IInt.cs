public interface IInteractionCostConfig
{
  float GlobalCooldown { get; }

  bool TryGetIntentCost(
    EInteractionIntentType intent,
    ItemCategoryData itemData,
    ETargetType targetType,
    out IntentCostEntry result);
}