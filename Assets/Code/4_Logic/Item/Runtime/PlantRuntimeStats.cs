using System.Collections.Generic;

public class PlantRuntimeStats
{
    public float Lifetime;
    public float Cooldown;
    public float CastTime;
    public float Duration;
    public float Range;
    public float AreaRadius;
    public float Damage;

    private List<IPlantModifier> _modifiers = new();

    public PlantRuntimeStats(IPlantItemData data, int level)
    {
        float levelMul = 1f + (level - 1) * 0.2f; // +20% per level เช่น

        Lifetime = data.BaseLifetime * levelMul;
        Cooldown = data.BaseCooldown;
        CastTime = data.BaseCastTime;
        Duration = data.BaseDuration * levelMul;
        Range = data.BaseRange * levelMul;
        AreaRadius = data.BaseAreaRadius * levelMul;
        Damage = data.BaseAreaRadius * levelMul;
    }

    public void AddModifier(IPlantModifier mod)
    {
        _modifiers.Add(mod);

        Lifetime *= mod.LifetimeMul;
        Duration *= mod.DurationMul;
        Range *= mod.RangeMul;
        Damage *= mod.DamageMul;
    }
}