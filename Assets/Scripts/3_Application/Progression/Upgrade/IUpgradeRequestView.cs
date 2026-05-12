using System;
using System.Collections.Generic;

public interface IUpgradeRequestView
{
  event Action<RequestBarViewModel> OnBarClicked;
  void Hide();
  void SetSlots(IReadOnlyList<RequestBarViewModel> bars);
}

public interface IUpgradeListener
{
  event Action OnOpenPopup;
  event Action OnClosePopup;
}

public interface IUpgradeManagerView
{
  void OnOpenUpgradePopup(string upgradeName, int gamekeyId);
  void ShowCraftPreview(AltarRecipeDefinition recipe, System.Action onConfirm);
  void HideCraftPreview();
}

public interface IProgressionView
{
  event Action OnFilled;
  void SetProgression(int currentLevel, float currentExp, float maxExp);
  void SetProgressionImmediate(int currentLevel, float currentExp, float maxExp);
}
