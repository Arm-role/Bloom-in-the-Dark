using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
  public static UpgradeManager Instance;

  private UpgradeData[] _allUpgrades;
  private IUpgradePopupUI _popup;

  private void Awake()
  {
    Instance = this;
  }

  public void Initialze(UpgradeData[] upgrades, IUpgradePopupUI popupUI)
  {
    _allUpgrades = upgrades;
    _popup = popupUI;
  }

  public List<UpgradeData> GetRandomUpgrades(int amount)
  {
    List<UpgradeData> result = new List<UpgradeData>();

    for (int i = 0; i < amount; i++)
    {
      int rand = Random.Range(0, _allUpgrades.Length);
      result.Add(_allUpgrades[rand]);
    }

    return result;
  }

  public void OpenUpgradePopup()
  {
    var upgrades = GetRandomUpgrades(3);

    _popup.Show(upgrades);
  }

  public void SelectUpgrade(UpgradeData upgrade)
  {
    //GlobalUpgradeSystem.Instance.AddUpgrade(upgrade);
    _popup.Hide();
    Debug.Log("Selected Upgrade: " + upgrade.upgradeName);
  }
}
