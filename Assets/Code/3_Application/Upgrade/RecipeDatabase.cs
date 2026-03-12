using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/RecipeDatabase")]
public class RecipeDatabase : ScriptableObject
{
  public List<UpgradeRequestDefinition> requests;
  public List<RecipeDefinition> recipes;
}