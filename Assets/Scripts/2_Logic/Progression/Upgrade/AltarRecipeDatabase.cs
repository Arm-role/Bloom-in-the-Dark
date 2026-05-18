using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Altar/RecipeDatabase")]
public class AltarRecipeDatabase : ScriptableObject
{
  public List<AltarRecipeDefinition> recipes;
}
