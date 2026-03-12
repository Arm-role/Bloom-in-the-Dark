using System.Collections.Generic;
using UnityEngine;

public class UpgradeManagerView : MonoBehaviour
{
  public List<UpgradeData> allUpgrades;
  public UpgradePopupUI popup;

  private void Start()
  {
    UpgradeManager.Instance.Initialze(allUpgrades.ToArray(), popup);
  }
}