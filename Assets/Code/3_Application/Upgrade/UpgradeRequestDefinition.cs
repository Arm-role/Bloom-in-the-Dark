using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class UpgradeRequestDefinition : ScriptableObject
{
  public string upgradeName;
  public List<Ingredient> ingredients;
}
