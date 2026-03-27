using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgradeDomain : IUpgradeContainer
{
  private readonly Dictionary<GameTag, List<StatModifier>> _activeUpgrades = new();
  private static readonly List<StatModifier> EmptyList = new();

  public event Action<GameTag, StatKey> onUpgrade;
  public void AddUpgrade(UpgradeData upgrade)
  {
    Debug.Log(upgrade.UpgradeName);

    if (!_activeUpgrades.TryGetValue(upgrade.Gamekey, out var list))
    {
      list = new List<StatModifier>();
      _activeUpgrades.Add(upgrade.Gamekey, list);
    }

    foreach (var modifier in upgrade.modifiers)
    {
      list.Add(modifier);
      Debug.Log(upgrade.Gamekey, modifier.StatKey);
      onUpgrade?.Invoke(upgrade.Gamekey, modifier.StatKey);
    }
  }

  public IEnumerable<StatModifier> GetUpgrades(GameTag itemKey)
  {
    if (_activeUpgrades.TryGetValue(itemKey, out var list))
      return list;

    Debug.Log("Return Empty");
    return EmptyList;
  }
}
