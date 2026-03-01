using System;

[Serializable]
public class IntentCostEntry
{
    public EInteractionIntentType Intent;
    public EItemCategory Category;
    public EItemRole ItemRole;
    public ETargetType TargetMask;
    
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