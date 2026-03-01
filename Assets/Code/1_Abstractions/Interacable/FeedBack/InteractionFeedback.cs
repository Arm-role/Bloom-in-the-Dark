public readonly struct InteractionFeedback
{
    public EInteractionIntentType IntentType { get; }

    public int EnergyCost { get; }
    public int ItemCost { get; }
    public float PlayerCooldown { get; }

    public bool HasCost =>
        EnergyCost > 0 || ItemCost > 0 || PlayerCooldown > 0f;

    public InteractionFeedback(
        EInteractionIntentType intentType,
        int energyCost = 0,
        int itemCost = 0,
        float playerCooldown = 0f)
    {
        IntentType = intentType;
        EnergyCost = energyCost;
        ItemCost = itemCost;
        PlayerCooldown = playerCooldown;
    }

    public static InteractionFeedback None(EInteractionIntentType intent)
        => new InteractionFeedback(intent);

    public static InteractionFeedback Consumed(
        EInteractionIntentType intent,
        int energyCost,
        int itemCost,
        float cooldown)
        => new InteractionFeedback(intent, energyCost, itemCost, cooldown);
}