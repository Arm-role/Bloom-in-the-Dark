public interface IInteractionCostConfig
{
  float GlobalCooldown { get; }

  bool TryGetIntentCost(
    EInteractionIntentType intent,
    IItemDefinition itemDefinition,
    ETargetType targetType,
    out IntentCostEntry result);
}