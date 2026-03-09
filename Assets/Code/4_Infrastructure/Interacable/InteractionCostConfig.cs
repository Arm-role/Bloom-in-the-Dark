using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Cost Config")]
public class InteractionCostConfig : ScriptableObject, IInteractionCostConfig
{
  [SerializeField] private float globalCooldown = 0.15f;
  [SerializeField] private List<IntentCostEntry> Entries;

  public float GlobalCooldown => globalCooldown;

  public bool TryGetIntentCost(
    EInteractionIntentType intent,
    IItemDefinition item,
    ETargetType targetType,
    out IntentCostEntry result)
  {

    foreach (var entry in Entries)
    {
      var rule = entry.MatchRule;

      if (!rule.Match(intent, item.Tags, targetType))
        continue;

      result = entry;
      return true;
    }

    result = null;
    return false;
  }
}