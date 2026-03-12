using System.Collections.Generic;

public interface IUpgradeContainer
{
  IEnumerable<StatModifier> GetUpgrades(GameTag itemKey);
}