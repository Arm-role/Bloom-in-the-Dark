using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Cost Config")]
public class InteractionCostConfig : ScriptableObject
{
  [SerializeField] private float globalCooldown = 0.15f;
  [SerializeField] private List<IntentCostEntry> Entries;

  public float GlobalCooldown => globalCooldown;

  public bool TryGetIntentCost(
    EInteractionIntentType intent,
    ItemCategoryData itemData,
    ETargetType targetType,
    out IntentCostEntry result)
  {
    foreach (var entry in Entries)
    {
      var matchRule = entry.MatchRule;

      if (matchRule.Intent != intent)
        continue;
      
      Debug.Log("PlantType " + matchRule.TargetMask + "!=" + targetType);
      
      if ((matchRule.TargetMask & targetType) == 0)
        continue;
      
      Debug.Log("Category " + matchRule.Category + "!=" + itemData.Category);
      
      if (matchRule.Category != EItemCategory.None &&
          matchRule.Category != itemData.Category)
        continue;
      
      Debug.Log("ItemRole " + matchRule.ItemRole + "!=" + itemData.ItemRole);

      if (matchRule.ItemRole != EItemRole.None &&
          matchRule.ItemRole != itemData.ItemRole)
        continue;

      result = entry;
      return true;
    }

    result = null;
    return false;
  }
}