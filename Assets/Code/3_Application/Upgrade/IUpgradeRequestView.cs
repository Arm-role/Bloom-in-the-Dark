using System.Collections.Generic;

public interface IUpgradeRequestView
{
  void Hide();
  void SetSlots(IReadOnlyList<RequestBarViewModel> bars);
}