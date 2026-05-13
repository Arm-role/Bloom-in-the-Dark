using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PhaseStatService : IStatService
{
  private readonly IGameStatConfig _gameStatConfig;
  private readonly IStatDatabase _statDatabase;
  private readonly IUpgradeContainer _upgradeContainer;

  public event Action<GameTag, StatKey> onUpgrade
  {
    add => _upgradeContainer.onUpgrade += value;
    remove => _upgradeContainer.onUpgrade -= value;
  }

  public PhaseStatService(
    IGameStatConfig config,
    IStatDatabase statDatabase,
    IUpgradeContainer upgradeContainer)
  {
    _gameStatConfig = config;
    _statDatabase = statDatabase;
    _upgradeContainer = upgradeContainer;
  }

  public float GetStat(StatKey key)
  {
    //Debug.Log("GetStat " + key.name);

    var breakdown = BuildBreakdown(key, _upgradeContainer.GetUpgrades(_gameStatConfig.Key));
    return ApplyRules(key, breakdown.GetFinal());
  }

  public float GetStatWithPreview(StatModifier previewModifier)
  {
    //Debug.Log("GetStatWithPreview " + previewModifier.StatKey.name);

    var mods = _upgradeContainer.GetUpgrades(_gameStatConfig.Key).Append(previewModifier);
    var breakdown =   BuildBreakdown(previewModifier.StatKey, mods);

    return ApplyRules(previewModifier.StatKey, breakdown.GetFinal());
  }

  public StatBreakdown GetBreakdown(StatKey key)
  {
    //Debug.Log("GetBreakdown " + key.name);

    return BuildBreakdown(key, _upgradeContainer.GetUpgrades(_gameStatConfig.Key));
  }

  private StatBreakdown BuildBreakdown(StatKey key, IEnumerable<StatModifier> modifiers)
  {
    StatBreakdown stat = new StatBreakdown
    {
      Base = _gameStatConfig.GetBaseStat(key),
      Multiplier = 1f
    };

    foreach (var mod in modifiers)
    {
      if (mod.StatKey != key)
        continue;

      stat.ApplyModifier(mod);
    }

    return stat;
  }

  private float ApplyRules(StatKey key, float value)
  {
    if (key == _statDatabase.Cooldown)
      return Mathf.Max(0.1f, value);

    return value;
  }
}
