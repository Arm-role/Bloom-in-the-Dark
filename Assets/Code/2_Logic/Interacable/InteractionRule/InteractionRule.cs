using System;

[Serializable]
public class InteractionRule
{
    public InputActionType Input;
    public InteractionPhase PhaseMask;
    public InteractionCondition Condition;
    public InteractionFallback Fallback;
    
    public EInteractionIntentType IntentType;
    public EItemStrategyType Strategy;
}