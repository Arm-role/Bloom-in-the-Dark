using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Cost Config")]
public class InteractionCostConfig : ScriptableObject
{
  [SerializeField] private float globalCooldown = 0.15f;
  [SerializeField] private List<IntentCostEntry> intentCosts;

  public float GlobalCooldown => globalCooldown;

  public bool TryGetIntentCost(
    EInteractionIntentType intent,
    string itemName,
    out IntentCostEntry entry)
  {
    entry = intentCosts.Find(x =>
      x.Intent == intent &&
      !string.IsNullOrEmpty(x.itemName) &&
      x.itemName == itemName);

    Debug.Log($"Trying to resolve item {itemName == string.Empty}");
    Debug.Log($"Trying to resolve item {itemName}");
    if (entry != null)
      return true;

    // 2️⃣ Intent-only fallback
    entry = intentCosts.Find(x =>
      x.Intent == intent &&
      string.IsNullOrEmpty(x.itemName));

    return entry != null;
  }
}