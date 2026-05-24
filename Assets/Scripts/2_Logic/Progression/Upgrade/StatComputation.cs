using System.Collections.Generic;
using UnityEngine;

// Shared rule logic for IStatService implementations (Item / Phase).
// Extract กัน ItemStatService กับ PhaseStatService duplicate body ของ BuildBreakdown + ApplyRules
public static class StatComputation
{
  public static StatBreakdown BuildBreakdown(
    StatKey key,
    float baseValue,
    IEnumerable<StatModifier> modifiers)
  {
    var stat = new StatBreakdown
    {
      Base = baseValue,
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

  public static float ApplyRules(IStatDatabase statDatabase, StatKey key, float value)
  {
    if (key == statDatabase.Cooldown)
      return Mathf.Max(0.1f, value);

    return value;
  }
}
