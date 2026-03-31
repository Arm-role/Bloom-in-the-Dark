using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradePopupUI : MonoBehaviour
{
  [SerializeField] private GameObject panel;
  [SerializeField] private TMP_Text upgradeNameTMP;
  [SerializeField] private UpgradeButtonUI[] buttons;

  public event Action<int> OnSelecetUpgrade;
  public void Show(string upgradeName, IStatPreviewContext context, IReadOnlyList<UpgradeData> upgrades)
  {
    panel.SetActive(true);

    upgradeNameTMP.text = upgradeName;

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