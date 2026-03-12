using System.Collections.Generic;

public interface IUpgradeRequestView
{
  void SetSlots(IReadOnlyList<RequestBarViewModel> bars);
}