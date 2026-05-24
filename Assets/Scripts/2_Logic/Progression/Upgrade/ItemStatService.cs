using System;
using System.Linq;
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
    var breakdown = StatComputation.BuildBreakdown(
      key, _item.Skill.GetBaseStat(key), _upgradeContainer.GetUpgrades(_item.Key));
    return StatComputation.ApplyRules(_statDatabase, key, breakdown.GetFinal());
  }

  public float GetStatWithPreview(StatModifier previewModifier)
  {
    var mods = _upgradeContainer.GetUpgrades(_item.Key).Append(previewModifier);
    var breakdown = StatComputation.BuildBreakdown(
      previewModifier.StatKey, _item.Skill.GetBaseStat(previewModifier.StatKey), mods);

    return StatComputation.ApplyRules(_statDatabase, previewModifier.StatKey, breakdown.GetFinal());
  }

  public StatBreakdown GetBreakdown(StatKey key)
  {
    return StatComputation.BuildBreakdown(
      key, _item.Skill.GetBaseStat(key), _upgradeContainer.GetUpgrades(_item.Key));
  }

  public IEnumerable<StatModifier> GetModifiers()
  {
    return _upgradeContainer.GetUpgrades(_item.Key);
  }
}
