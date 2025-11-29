using System.Collections.Generic;

public class PlantItemInstance : IItemInstance
{
    public IItemData Data { get; }
    public IPlantItemData PlantData => (IPlantItemData)Data;

    public float CurrentLifetime { get; private set; }
    public int Level { get; private set; }

    private readonly List<StatModifier> _modifiers = new();

    public PlantItemInstance(IItemData itemData)
    {
        Data = itemData;
        Level = 1;
        CurrentLifetime = PlantData.BaseLifetime;
    }

    public void AddModifier(StatModifier mod) => _modifiers.Add(mod);

    public IEnumerable<StatModifier> GetModifiers() => _modifiers;

    public void AddLevel(int amount = 1)
    {
        Level += amount;
    }

    // ----------------------------
    // 🔥 Generic Stat Compute Func
    // ----------------------------
    public float GetStat(EStatType stat)
    {
        float baseValue = PlantData.GetBaseStat(stat);
        float perLevel = PlantData.GetPerLevelStat(stat);

        float result = baseValue + perLevel * (Level - 1);

        foreach (var mod in _modifiers)
        {
            if (mod.Stat != stat) continue;
            result = ApplyModifier(result, mod);
        }

        return result;
    }

    private float ApplyModifier(float value, StatModifier mod)
    {
        return mod.ModifierType switch
        {
            EModifierType.Add => value + mod.Value,
            EModifierType.Multiply => value * (1 + mod.Value),
            _ => value
        };
    }

    // ----------------------------
    // 🔥 Convenience Accessors  
    // ----------------------------

    public float Damage => GetStat(EStatType.Damage);
    public float Range => GetStat(EStatType.Range);
    public float AreaRadius => GetStat(EStatType.AreaRadius);
    public float ScaledLifetime => GetStat(EStatType.Lifetime);
    public float KnockForce => GetStat(EStatType.KnockForce);
    public float KnockDuration => GetStat(EStatType.KnockDuration);

}
