using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Interaction/Capability")]
public class ItemInteractionCapability : ScriptableObject, IItemInteractionCapability
{
    public List<InteractionRule> InteractionRules;
    public List<PreviewRule> PreviewRules;
   
    public bool TryGetInteractionRule(
        InputActionType input,
        InteractionPhase phase,
        InteractionConditionContext ctx,
        out InteractionRule rule)
    {
        foreach (var r in InteractionRules)
        {
            if (r.Input != input) continue;
            if (!r.PhaseMask.HasFlag(phase)) continue;
            if (!r.Condition.IsMet(ctx)) continue;

            rule = r;
            return true;
        }

        rule = null;
        return false;
    }
    public IEnumerable<PreviewRule> GetPreviewRules(
        InputActionType input,
        InteractionPhase phase,
        ItemSelectionPhase selection)
    {
        foreach (var pr in PreviewRules)
        {
            if (pr.SelectionPhase != ItemSelectionPhase.None &&
                pr.SelectionPhase != selection)
                continue;

            if (!input.HasFlag(pr.Input))
                continue;

            if (!pr.PhaseMask.HasFlag(phase))
                continue;
            
            yield return pr;
        }
    }
}