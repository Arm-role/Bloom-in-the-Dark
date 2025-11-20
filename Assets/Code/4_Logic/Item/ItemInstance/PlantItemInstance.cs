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
        CurrentLifetime = PlantData.BaseLifetime;
        Level = 1;
    }

    public void AddModifier(StatModifier mod)
    {
        _modifiers.Add(mod);
    }

    public IEnumerable<StatModifier> GetModifiers() => _modifiers;

    public void AddLevel(int amount = 1)
    {
        Level += amount;
    }

    public float GetDamage()
    {
        float result = PlantData.BaseDamage;

        // Scale by Level
        result += PlantData.DamagePerLevel * (Level - 1);

        foreach (var mod in _modifiers)
            if (mod.Stat == EStatType.Damage)
                result = ApplyModifier(result, mod);

        return result;
    }

    public float GetAreaRadius()
    {
        float result = PlantData.BaseAreaRadius;

        result += PlantData.AreaRadiusPerLevel * (Level - 1);

        foreach (var mod in _modifiers)
            if (mod.Stat == EStatType.AreaRadius)
                result = ApplyModifier(result, mod);

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
}
