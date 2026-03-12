using System.Collections.Generic;
using UnityEngine;

public class RecipeUsageLookup : MonoBehaviour
{
  [SerializeField] private RecipeDatabase recipeDatabase;

  private Dictionary<ItemKey, List<RecipeDefinition>> _usageMap = new();
  private IUpgradeRequestView _view;

  private void Start()
  {
    BuildRecipe(recipeDatabase.recipes);
  }
  private void Update()
  {
    if (Input.GetKey(KeyCode.I))
    {
      var requests = recipeDatabase.requests;
      var bars = new List<RequestBarViewModel>();

      foreach (var request in requests)
      {
        var bar = new RequestBarViewModel
        {
          upgradeName = request.upgradeName,
          slotViewModels = new List<RequestSlotViewModel>()
        };

        foreach (var ingredient in request.ingredients)
        {
          bar.slotViewModels.Add(new RequestSlotViewModel
          {
            ItemId = ingredient.item.RuntimeTag.Hash,
            Amount = ingredient.amount
          });
        }

        bars.Add(bar);
      }

      _view.SetSlots(bars);
    }
  }
  public void Initialize(IUpgradeRequestView requestView)
  {
    _view = requestView;
  }

  public void BuildRecipe(IEnumerable<RecipeDefinition> recipes)
  {
    foreach (var recipe in recipes)
    {
      foreach (var ingredient in recipe.ingredients)
      {
        if (!_usageMap.TryGetValue(ingredient.item, out var list))
        {
          list = new List<RecipeDefinition>();
          _usageMap.Add(ingredient.item, list);
        }

        list.Add(recipe);
      }
    }
  }

  public IEnumerable<RecipeDefinition> GetRecipesUsing(ItemKey item)
  {
    if (_usageMap.TryGetValue(item, out var list))
      return list;

    return System.Array.Empty<RecipeDefinition>();
  }
}