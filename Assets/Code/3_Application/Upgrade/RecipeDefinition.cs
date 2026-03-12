using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class RecipeDefinition : ScriptableObject
{
  public ItemKey upgradeName;
  public List<Ingredient> ingredients;
}

[System.Serializable]
public struct Ingredient
{
  public ItemKey item;
  public int amount;
}
