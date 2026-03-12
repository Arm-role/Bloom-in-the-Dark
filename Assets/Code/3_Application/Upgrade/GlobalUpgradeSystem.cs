using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgradeSystem : MonoBehaviour, IUpgradeContainer
{
  public static GlobalUpgradeSystem Instance;

  private readonly Dictionary<GameTag, List<StatModifier>> _activeUpgrades = new();
  private static readonly List<StatModifier> EmptyList = new();
  void Awake()
  {
    Instance = this;
  }

  public void AddUpgrade(UpgradeData upgrade)
  {
    if (!_activeUpgrades.TryGetValue(upgrade.itemKey, out var list))
    {
      list = new List<StatModifier>();
      _activeUpgrades.Add(upgrade.itemKey, list);
    }

    list.Add(new StatModifier
    {
      StatKey = upgrade.statKey,
      ModifierType = upgrade.modifierType,
      Value = upgrade.value
    });
  }

  public IEnumerable<StatModifier> GetUpgrades(GameTag itemKey)
  {
    if (_activeUpgrades.TryGetValue(itemKey, out var list))
      return list;

    return EmptyList;
  }
}