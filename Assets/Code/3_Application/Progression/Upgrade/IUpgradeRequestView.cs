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
  event Action OnOpenUpgradePopup;
  event Action<int> OnSelectUpgrade;
}

public interface IUpgradeManagerView
{
  public void OnOpenPopup(int gamekeyId);
}
