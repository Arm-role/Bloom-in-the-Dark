using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/UpgradeRequest")]
public class UpgradeRequestDefinition : ScriptableObject
{
  public string upgradeName;
  public List<Ingredient> ingredients;
}

[Serializable]
public struct Ingredient
{
  public ItemKey item;
  public int amount;
}