using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Upgrade/UpgradeRequest")]
public class UpgradeRequestDefinition : ScriptableObject
{
  [SerializeField] private string upgradeName;
  [SerializeField] private GameKeyAsset upgradeItemKey;
  [SerializeField] private List<Ingredient> ingredients;

  public string UpgradeName => upgradeName;
  public int GameKeyId => upgradeItemKey.RuntimeTag.Hash;
  public IReadOnlyList<Ingredient> Ingredients => ingredients;
}

[Serializable]
public struct Ingredient
{
  public ItemKey item;
  public int amount;
}
