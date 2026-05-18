using System.Collections.Generic;

public static class AltarRecipeQuery
{
  public static bool TryGetRecipesUsingItems(
        List<int> itemIds,
        List<AltarRecipeDefinition> recipes,
        out List<AltarRecipeDefinition> matching)
  {
    matching = new List<AltarRecipeDefinition>();

    if (recipes == null) return false;

    foreach (var recipe in recipes)
    {
      if (recipe == null) continue;

      bool match = true;

      foreach (var itemId in itemIds)
      {
        bool found = false;

        foreach (var ingredient in recipe.Ingredients)
        {
          if (ingredient.item.RuntimeTag.Hash == itemId)
          {
            found = true;
            break;
          }
        }

        if (!found)
        {
          match = false;
          break;
        }
      }

      if (match)
        matching.Add(recipe);
    }

    return matching.Count > 0;
  }
}
