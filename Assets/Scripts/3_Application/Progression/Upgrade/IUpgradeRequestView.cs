using System;
using System.Collections.Generic;

public interface IUpgradeListener
{
  event Action OnOpenPopup;
  event Action OnClosePopup;
}

public interface IUpgradeManagerView
{
  void OnOpenUpgradePopup(string upgradeName, int gamekeyId);
  void ShowRecipePreview(AltarRecipeDefinition recipe, System.Action onConfirm);
  void HideCraftPreview();
}

public interface IProgressionView
{
  event Action OnFilled;
  void SetProgression(int currentLevel, float currentExp, float maxExp);
  void SetProgressionImmediate(int currentLevel, float currentExp, float maxExp);
}

public interface IAltarRecipeSuggestionView
{
  void ShowSuggestions(List<AltarRecipeDefinition> recipes, int startSlot = 0);
  void HideAll();
}
