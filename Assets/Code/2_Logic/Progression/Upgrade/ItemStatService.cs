using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ItemStatService : IStatService
{
  private readonly IItemDefinition _item;
  private readonly IStatDatabase _statDatabase;
  private readonly IUpgradeContainer _upgradeContainer;

  public event Action<GameTag, StatKey> onUpgrade
  {
    add => _upgradeContainer.onUpgrade += value;
    remove => _upgradeContainer.onUpgrade -= value;
  }
  public ItemStatService(
    IItemDefinition item,
    IStatDatabase statDatabase,
    IUpgradeContainer upgradeContainer)
  {
    _item = item;
    _statDatabase = statDatabase;
    _upgradeContainer = upgradeContainer;
  }

  public float GetStat(StatKey key)
  {
    var breakdown = BuildBreakdown(key, _upgradeContainer.GetUpgrades(_item.Key));
    return ApplyRules(key, breakdown.GetFinal());
  }

  public float GetStatWithPreview(StatModifier previewModifier)
  {
    var mods = _upgradeContainer.GetUpgrades(_item.Key).Append(previewModifier);
    var breakdown = BuildBreakdown(previewModifier.StatKey, mods);

    return ApplyRules(previewModifier.StatKey, breakdown.GetFinal());
  }

  public StatBreakdown GetBreakdown(StatKey key)
  {
    return BuildBreakdown(key, _upgradeContainer.GetUpgrades(_item.Key));
  }

  private StatBreakdown BuildBreakdown(StatKey key, IEnumerable<StatModifier> modifiers)
  {
    StatBreakdown stat = new StatBreakdown
    {
      Base = _item.Skill.GetBaseStat(key),
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


  public IEnumerable<StatModifier> GetModifiers()
  {
    return _upgradeContainer.GetUpgrades(_item.Key); ;
  }
}
