using System.Collections.Generic;
using UnityEngine;

public class ItemInstanceBase : IItemInstance
{
  public IItemDefinition Data { get; }
  public int Level { get; private set; }

  private readonly IUpgradeContainer _upgradeContainer;

  public ItemInstanceBase(IItemDefinition data, IUpgradeContainer upgradeContainer, int level = 1)
  {
    Data = data;
    Level = level;
    _upgradeContainer = upgradeContainer;
  }

  public void AddLevel(int amount = 1)
  {
    Level += amount;
  }

  public IEnumerable<StatModifier> GetModifiers()
  {
    return _upgradeContainer.GetUpgrades(Data.Key); ;
  }

  public float GetStat(StatKey key)
  {
    float baseValue = Data.Skill.GetBaseStat(key);

    float add = 0f;
    float percentAdd = 0f;
    float multiply = 1f;

    bool hasOverride = false;
    float overrideValue = 0f;

    foreach (var mod in _upgradeContainer.GetUpgrades(Data.Key))
    {
      if(mod.StatKey != key)
            continue;

      switch (mod.ModifierType)
      {
        case EModifierType.Add:
          add += mod.Value;
          break;

        case EModifierType.PercentAdd:
          percentAdd += mod.Value;
          break;

        case EModifierType.Multiply:
          multiply *= mod.Value;
          break;

        case EModifierType.Override:
          hasOverride = true;
          overrideValue = mod.Value;
          break;
      }
    }

    float result;

    if (hasOverride)
    {
      result = overrideValue;
    }
    else
    {
      result = ((baseValue + add) * (1f + percentAdd)) * multiply;
    }

    if (key.Id == "CooldownKey")
      result = Mathf.Max(0.1f, result);

    return result;
  }
}
