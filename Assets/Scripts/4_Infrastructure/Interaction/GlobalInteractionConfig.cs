using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/GlobalInteractionConfig")]
public class GlobalInteractionConfig : ScriptableObject, IGlobalInteractionConfig
{
    [Serializable]
    public struct TagIntentRule
    {
        public GameTagAsset Tag;
        public EInteractionIntentType Intent;
    }

    [SerializeField] private List<TagIntentRule> _rules = new();
    [SerializeField] private EInteractionIntentType _defaultIntent = EInteractionIntentType.Harvest;

    public EInteractionIntentType Resolve(IItemInstance item)
    {
        if (item != null)
        {
            foreach (var rule in _rules)
            {
                if (rule.Tag != null && item.Data.HasTag(rule.Tag.RuntimeTag))
                    return rule.Intent;
            }
        }

        return _defaultIntent;
    }
}
