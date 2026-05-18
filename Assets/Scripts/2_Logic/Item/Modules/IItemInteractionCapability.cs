using System.Collections.Generic;

public interface IItemInteractionCapability : IPreviewProvider
{
  bool TryGetInteractionRule(InputActionType input,
    InteractionPhase phase,
    InteractionConditionContext context,
    out InteractionRule rule);
}