using System;
using System.Linq;

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
    var breakdown = StatComputation.BuildBreakdown(
      key, _gameStatConfig.GetBaseStat(key), _upgradeContainer.GetUpgrades(_gameStatConfig.Key));
    return StatComputation.ApplyRules(_statDatabase, key, breakdown.GetFinal());
  }

  public float GetStatWithPreview(StatModifier previewModifier)
  {
    var mods = _upgradeContainer.GetUpgrades(_gameStatConfig.Key).Append(previewModifier);
    var breakdown = StatComputation.BuildBreakdown(
      previewModifier.StatKey, _gameStatConfig.GetBaseStat(previewModifier.StatKey), mods);

    return StatComputation.ApplyRules(_statDatabase, previewModifier.StatKey, breakdown.GetFinal());
  }

  public StatBreakdown GetBreakdown(StatKey key)
  {
    return StatComputation.BuildBreakdown(
      key, _gameStatConfig.GetBaseStat(key), _upgradeContainer.GetUpgrades(_gameStatConfig.Key));
  }
}
