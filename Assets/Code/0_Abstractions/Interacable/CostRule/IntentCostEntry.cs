using System;

[Serializable]
public class IntentCostEntry
{
    public EInteractionIntentType Intent;

    public int EnergyCost;
    public int ItemCost;
    public float Cooldown;
    
    public InteractionFeedback ToFeedback()
    {
        return new InteractionFeedback(
            Intent,
            EnergyCost,
            ItemCost,
            Cooldown
        );
    }
}