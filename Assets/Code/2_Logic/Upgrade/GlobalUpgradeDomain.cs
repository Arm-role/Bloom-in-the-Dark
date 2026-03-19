using System.Collections.Generic;

public class GlobalUpgradeDomain : IUpgradeContainer
{
  private readonly Dictionary<GameTag, List<StatModifier>> _activeUpgrades = new();
  private static readonly List<StatModifier> EmptyList = new();

  public void AddUpgrade(UpgradeData upgrade)
  {
    if (!_activeUpgrades.TryGetValue(upgrade.itemKey, out var list))
    {
      list = new List<StatModifier>();
      _activeUpgrades.Add(upgrade.itemKey, list);
    }

    foreach (var modifier in upgrade.modifiers)
    {
      list.Add(modifier);
    }
  }

  public IEnumerable<StatModifier> GetUpgrades(GameTag itemKey)
  {
    if (_activeUpgrades.TryGetValue(itemKey, out var list))
      return list;

    return EmptyList;
  }
}