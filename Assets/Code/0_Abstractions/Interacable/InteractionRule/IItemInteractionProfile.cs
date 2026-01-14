using System.Collections.Generic;

public interface IItemInteractionProfile
{
    bool TryGetInteractionRule(InputActionType input,
        InteractionPhase phase,
        InteractionConditionContext context,
        out InteractionRule rule);

    IEnumerable<PreviewRule> GetPreviewRules(
        InputActionType input,
        InteractionPhase phase,
        ItemSelectionPhase selection);
}