using System;
using UnityEngine;
using System.Collections.Generic;

public class GlobalUpgradeDomain : IUpgradeContainer
{
  private readonly Dictionary<GameTag, List<StatModifier>> _activeUpgrades = new();
  private static readonly List<StatModifier> EmptyList = new();

  private readonly TagUpgradeThresholdService _thresholdService;

  public event Action<GameTag, StatKey> onUpgrade;

  public GlobalUpgradeDomain(TagUpgradeThresholdService thresholdService)
  {
    _thresholdService = thresholdService;

    _thresholdService.OnThresholdReward += AddUpgrade;
  }

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

    _thresholdService.RegisterUpgrade(upgrade.Gamekey);
  }

  public IEnumerable<StatModifier> GetUpgrades(GameTag itemKey)
  {
    if (_activeUpgrades.TryGetValue(itemKey, out var list))
      return list;

    //Debug.Log("Return Empty");
    return EmptyList;
  }
}
