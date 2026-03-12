using System.Collections.Generic;
using UnityEngine;

public class UpgradePopupUI : MonoBehaviour, IUpgradePopupUI
{
  public GameObject panel;

  public UpgradeButtonUI[] buttons;

  public void Show(List<UpgradeData> upgrades)
  {
    panel.SetActive(true);

    for (int i = 0; i < buttons.Length; i++)
    {
      buttons[i].Setup(upgrades[i]);
    }
  }

  public void Hide()
  {
    panel.SetActive(false);
  }
}