using System;

[Serializable]
public class IntentCostEntry
{
    public EInteractionIntentType Intent;

    public int EnergyCost;
    public int ItemCost;
    public float Cooldown;
    
   public InteractionFeedback ToFeedback(
        InteractionOutcome outcome)
    {
        if (outcome != InteractionOutcome.Consumed)
            return InteractionFeedback.None(Intent);

        return InteractionFeedback.Consumed(
            Intent,
            EnergyCost,
            ItemCost,
            Cooldown);
    }
}