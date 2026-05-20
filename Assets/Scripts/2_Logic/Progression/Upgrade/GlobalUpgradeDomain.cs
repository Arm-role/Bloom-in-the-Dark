using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgradeDomain : IUpgradeContainer
{
  private readonly Dictionary<GameTag, List<StatModifier>> _activeUpgrades = new();
  private static readonly List<StatModifier> EmptyList = new();

  private readonly TagUpgradeThresholdService _thresholdService;

  public event Action<GameTag, StatKey> onUpgrade;

  public event Action<UpgradeData> OnThresholdReward
  {
    add    => _thresholdService.OnThresholdReward += value;
    remove => _thresholdService.OnThresholdReward -= value;
  }

  public event Action OnAltarPhaseAdvance
  {
    add    => _thresholdService.OnAltarPhaseAdvance += value;
    remove => _thresholdService.OnAltarPhaseAdvance -= value;
  }

  public GlobalUpgradeDomain(TagUpgradeThresholdService thresholdService)
  {
    _thresholdService = thresholdService;

    _thresholdService.OnThresholdReward += ApplyThresholdReward;
  }

  public void AddUpgrade(UpgradeData upgrade)
  {
    Debug.Log(upgrade.UpgradeName);

    ApplyModifiers(upgrade);
    _thresholdService.RegisterUpgrade(upgrade.Gamekey);

  }

  private void ApplyThresholdReward(UpgradeData upgrade)
  {
    ApplyModifiers(upgrade);
  }

  private void ApplyModifiers(UpgradeData upgrade)
  {
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

    //Debug.Log("Return Empty");
    return EmptyList;
  }
}
