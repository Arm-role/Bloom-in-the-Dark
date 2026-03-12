using System.Collections.Generic;

public interface IUpgradePopupUI
{
  void Hide();
  void Show(List<UpgradeData> upgrades);
}