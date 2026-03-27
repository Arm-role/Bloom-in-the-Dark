using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePopupUI : MonoBehaviour
{
  public GameObject panel;

  public UpgradeButtonUI[] buttons;

  public event Action<int> OnSelecetUpgrade;
  public void Show(IStatPreviewContext context, IReadOnlyList<UpgradeData> upgrades)
  {
    panel.SetActive(true);

    for (int i = 0; i < buttons.Length; i++)
    {
      buttons[i].OnClicked -= OnClickedUpgrade;
      buttons[i].Setup(i, context, upgrades[i]);
      buttons[i].OnClicked += OnClickedUpgrade;
    }
  }

  private void OnClickedUpgrade(int i)
  {
    OnSelecetUpgrade?.Invoke(i);
  }

  public void Hide()
  {
    panel.SetActive(false);
  }
}