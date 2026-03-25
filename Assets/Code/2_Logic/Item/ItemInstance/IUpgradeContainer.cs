using System;
using System.Collections.Generic;

public interface IUpgradeContainer
{
  event Action<GameTag, StatKey> onUpgrade;
  IEnumerable<StatModifier> GetUpgrades(GameTag key);
}