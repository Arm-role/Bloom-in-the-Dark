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
