using System;
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
      if (entry.Intent != intent)
        continue;
      
      Debug.Log("PlantType " + entry.TargetMask + "!=" + targetType);
      
      if ((entry.TargetMask & targetType) == 0)
        continue;
      
      Debug.Log("Category " + entry.Category + "!=" + itemData.Category);
      
      if (entry.Category != EItemCategory.None &&
          entry.Category != itemData.Category)
        continue;
      
      Debug.Log("ItemRole " + entry.ItemRole + "!=" + itemData.ItemRole);

      if (entry.ItemRole != EItemRole.None &&
          entry.ItemRole != itemData.ItemRole)
        continue;

      result = entry;
      return true;
    }

    result = null;
    return false;
  }
}