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
    var stat = GetStatBreakdown(key);

    float result = stat.GetFinal();

    if (key.Id == "CooldownKey")
      result = Mathf.Max(0.1f, result);

    return result;
  }

  public StatBreakdown GetStatBreakdown(StatKey key)
  {
    StatBreakdown stat = new StatBreakdown
    {
      Base = Data.Skill.GetBaseStat(key),
      Multiplier = 1f
    };

    foreach (var mod in _upgradeContainer.GetUpgrades(Data.Key))
    {
      if (mod.StatKey != key)
        continue;

      switch (mod.ModifierType)
      {
        case EModifierType.Add:
          stat.Flat += mod.Value;
          break;

        case EModifierType.PercentAdd:
          stat.Percent += mod.Value;
          break;

        case EModifierType.Multiply:
          stat.Multiplier *= mod.Value;
          break;

        case EModifierType.Override:
          stat.HasOverride = true;
          stat.OverrideValue = mod.Value;
          break;
      }
    }

    return stat;
  }
}

public struct StatBreakdown
{
  public float Base;
  public float Flat;
  public float Percent;
  public float Multiplier;
  public bool HasOverride;
  public float OverrideValue;

  public float GetFinal()
  {
    if (HasOverride)
      return OverrideValue;

    return ((Base + Flat) * (1f + Percent)) * Multiplier;
  }

  public StatBreakdown ApplyModifier(StatModifier mod)
  {
    switch (mod.ModifierType)
    {
      case EModifierType.Add:
        Flat += mod.Value;
        break;

      case EModifierType.PercentAdd:
        Percent += mod.Value;
        break;

      case EModifierType.Multiply:
        Multiplier *= mod.Value;
        break;

      case EModifierType.Override:
        HasOverride = true;
        OverrideValue = mod.Value;
        break;
    }

    return this;
  }
}
