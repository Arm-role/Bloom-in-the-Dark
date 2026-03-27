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
  public void OnOpenUpgradePopup(int gamekeyId);
}
