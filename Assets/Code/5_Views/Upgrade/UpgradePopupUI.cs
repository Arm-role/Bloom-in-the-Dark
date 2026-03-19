using System.Collections.Generic;
using UnityEngine;

public class UpgradePopupUI : MonoBehaviour, IUpgradePopupUI
{
  public GameObject panel;

  public UpgradeButtonUI[] buttons;
  public List<UpgradeData> _upgrades;
  public void Show(List<UpgradeData> upgrades)
  {
    panel.SetActive(true);

    _upgrades = new List<UpgradeData>(upgrades);

    for (int i = 0; i < buttons.Length; i++)
    {
      buttons[i].Setup(i, upgrades[i]);
      buttons[i].OnClicked += OnClickedUpgrade;
    }
  }

  private void OnClickedUpgrade(int i)
  {
    UpgradeManager.Instance.SelectUpgrade(_upgrades[i]);
  }

  public void Hide()
  {
    panel.SetActive(false);
  }
}