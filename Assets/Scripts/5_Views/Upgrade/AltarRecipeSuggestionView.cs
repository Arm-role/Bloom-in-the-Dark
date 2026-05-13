using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarRecipeSuggestionView : MonoBehaviour, IAltarRecipeSuggestionView
{
  [SerializeField] private Image[] _resultSlots;

  private IItemIconProvider _iconProvider;

  public void Initialize(IItemIconProvider iconProvider)
  {
    _iconProvider = iconProvider;
    HideAll();
  }

  public void ShowSuggestions(List<AltarRecipeDefinition> recipes, int startSlot = 0)
  {
    for (int i = 0; i < _resultSlots.Length; i++)
    {
      int recipeIndex = i - startSlot;
      if (i < startSlot || recipeIndex >= recipes.Count)
      {
        _resultSlots[i].enabled = false;
      }
      else
      {
        _resultSlots[i].sprite = _iconProvider.GetIcon(recipes[recipeIndex].ResultItemId);
        _resultSlots[i].enabled = true;
      }
    }
  }

  public void HideAll()
  {
    foreach (var slot in _resultSlots)
      slot.enabled = false;
  }
}
