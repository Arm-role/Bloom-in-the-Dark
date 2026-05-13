using System;
using System.Collections.Generic;
using UnityEngine;

public class TagUpgradeThresholdService
{
  private readonly Dictionary<GameTag, int> _upgradeCounts = new();
  private readonly Dictionary<GameTag, HashSet<int>> _triggeredThresholds = new();

  private readonly Dictionary<GameTag, UpgradeThresholdConfig> _configs;

  public event Action<UpgradeData> OnThresholdReward;

  public TagUpgradeThresholdService(List<UpgradeThresholdConfig> configs)
  {
    _configs = new();

    foreach (var config in configs)
    {
      _configs[config.tag] = config;
    }
  }

  public void RegisterUpgrade(GameTag tag)
  {
    if (!_upgradeCounts.ContainsKey(tag))
      _upgradeCounts[tag] = 0;

    _upgradeCounts[tag]++;

    CheckThreshold(tag);
  }

  private void CheckThreshold(GameTag tag)
  {
    if (!_configs.TryGetValue(tag, out var config))
      return;

    int count = _upgradeCounts[tag];

    if (!_triggeredThresholds.ContainsKey(tag))
      _triggeredThresholds[tag] = new HashSet<int>();

    foreach (var threshold in config.thresholds)
    {
      if (count < threshold.count)
        continue;

      if (_triggeredThresholds[tag].Contains(threshold.count))
        continue;

      _triggeredThresholds[tag].Add(threshold.count);

      Debug.Log($"Threshold reached {tag} : {threshold.count}");

      OnThresholdReward?.Invoke(threshold.reward);
    }
  }
}