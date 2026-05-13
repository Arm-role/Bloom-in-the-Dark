using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Altar/Recipe")]
public class AltarRecipeDefinition : ScriptableObject
{
  public const int MaxSlots = 6;

  [SerializeField] private List<Ingredient> ingredients;
  [SerializeField] private ItemKey resultItem;

  public string Name       => name; // Unity ScriptableObject asset name
  public ItemKey ResultItem  => resultItem;
  public int ResultItemId    => resultItem.RuntimeTag.Hash;
  public IReadOnlyList<Ingredient> Ingredients => ingredients;

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
  [Range(1, AltarRecipeDefinition.MaxSlots)] public int amount;
}
