using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Upgrade/UpgradeRequest")]
public class UpgradeRequestDefinition : ScriptableObject
{
  public const int MaxSlots = 6;

  [SerializeField] private string upgradeName;
  [SerializeField] private GameKeyAsset upgradeItemKey;
  [SerializeField] private List<Ingredient> ingredients;

  public string UpgradeName => upgradeName;
  public int GameKeyId => upgradeItemKey.RuntimeTag.Hash;
  public IReadOnlyList<Ingredient> Ingredients => ingredients;

  // Total altar slots this recipe occupies. Must not exceed MaxSlots.
  public int TotalSlots => ingredients?.Sum(i => i.amount) ?? 0;

#if UNITY_EDITOR
  private void OnValidate()
  {
    int total = TotalSlots;
    if (total > MaxSlots)
      Debug.LogWarning($"[{name}] Total ingredient slots ({total}) exceeds altar max ({MaxSlots}).", this);
  }
#endif
}

[Serializable]
public struct Ingredient
{
  public ItemKey item;
  [Range(1, UpgradeRequestDefinition.MaxSlots)] public int amount;
}
