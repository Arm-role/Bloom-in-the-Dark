using System.Collections.Generic;

public class ItemInstanceBase : IItemInstance
{
    public IItemDefinition Data { get; }
    public int Level { get; private set; }
    
    private readonly Dictionary<StatKey, float> _flatModifiers = new();
    private readonly Dictionary<StatKey, float> _multipliers = new();
    
    private readonly List<StatModifier> _modifiers = new();

    public ItemInstanceBase(IItemDefinition data, int level = 1)
    {
        Data = data;
        Level = level;
    }

    public void AddLevel(int amount = 1)
    {
        Level += amount;
    }

    public void AddFlatModifier(StatKey key, float value)
    {
        if (_flatModifiers.ContainsKey(key))
            _flatModifiers[key] += value;
        else
            _flatModifiers[key] = value;
    }

    public void AddMultiplier(StatKey key, float value)
    {
        if (_multipliers.ContainsKey(key))
            _multipliers[key] += value;
        else
            _multipliers[key] = value;
    }

    public float GetFlatBonus(StatKey key)
        => _flatModifiers.TryGetValue(key, out var v) ? v : 0f;

    public float GetMultiplier(StatKey key)
        => _multipliers.TryGetValue(key, out var v) ? v : 0f;
    
    
    public float ApplyModifier(float value, StatModifier mod)
    {
        return mod.ModifierType switch
        {
            EModifierType.Add => value + mod.Value,
            EModifierType.Multiply => value * (1 + mod.Value),
            _ => value
        };
    }
    

    public void AddModifier(StatModifier mod) => _modifiers.Add(mod);
    public IEnumerable<StatModifier> GetModifiers() => _modifiers;
}