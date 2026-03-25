using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/UpgradeMatchService")]
public class UpgradeMatchService : ScriptableObject
{
  [Header("Config")]
  [SerializeField] private UpgradeData[] upgrades;

  private Dictionary<int, List<UpgradeData>> _upgradeDic;

  private bool _initialized;

  #region Initialization

  private void OnEnable()
  {
    Initialize();
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    Initialize();
  }
#endif

  private void Initialize()
  {
    BuildData();
    _initialized = true;
  }

  private void BuildData()
  {
    _upgradeDic = new Dictionary<int, List<UpgradeData>>();

    if (upgrades == null) return;

    foreach (var upgrade in upgrades)
    {
      if (upgrade == null) continue;

      int itemId = upgrade.Gamekey.Hash;

      if (!_upgradeDic.TryGetValue(itemId, out var list))
      {
        list = new List<UpgradeData>();
        _upgradeDic[itemId] = list;
      }

      list.Add(upgrade);
    }
  }

  #endregion

  #region Public API

  public bool TryGetUpgradesData(
    int gamekeyId,
    int amount,
    out List<UpgradeData> upgradeDatas)
  {
    upgradeDatas = null;

    if (!_initialized || _upgradeDic == null)
      Initialize();

    if (_upgradeDic == null)
    {
      Debug.LogError("UpgradeMatchService: Dictionary not initialized");
      return false;
    }

    if (!_upgradeDic.TryGetValue(gamekeyId, out var list) || list == null || list.Count == 0)
      return false;

    var pool = new List<UpgradeData>(list);
    var result = new List<UpgradeData>();

    int count = Mathf.Min(amount, pool.Count);

    for (int i = 0; i < count; i++)
    {
      int rand = UnityEngine.Random.Range(0, pool.Count);

      result.Add(pool[rand]);
      pool.RemoveAt(rand);
    }

    upgradeDatas = result;
    return true;
  }

  #endregion
}