using System.Collections.Generic;

public class ItemInstanceBase : IItemInstance
{
    public IItemData Data { get; }

    protected int Level { get; private set; }
    protected readonly List<StatModifier> _modifiers = new();

    public ItemInstanceBase(IItemData itemData, int level = 1)
    {
        Data = itemData;
        Level = level;
    }
    
    // ----------------------------
    // 🔥 Generic Stat Compute Func
    // ----------------------------

    public virtual bool HasStat(EItemStatType stat)
    {
        return Data.SupportsStat(stat);
    }

    public virtual float GetStat(EItemStatType stat)
    {
        if (!HasStat(stat))
            return 0f;

        float baseValue = Data.GetBaseStat(stat);
        float perLevel = Data.GetPerLevelStat(stat);

        float result = baseValue + perLevel * (Level - 1);

        foreach (var mod in _modifiers)
            if (mod.ItemStat == stat)
                result = ApplyModifier(result, mod);

        return result;
    }
    
    protected virtual float ApplyModifier(float value, StatModifier mod)
    {
        return mod.ModifierType switch
        {
            EModifierType.Add => value + mod.Value,
            EModifierType.Multiply => value * (1 + mod.Value),
            _ => value
        };
    }
    
    public virtual bool HasProperty(EItemProperty property)
    {
        return Data.SupportsProperty(property);
    }
    
    public virtual T GetProperty<T>(EItemProperty property)
    {
        if (!HasProperty(property)) 
            return default;
        
        return Data.GetProperty<T>(property);
    }

    public void AddModifier(StatModifier mod) => _modifiers.Add(mod);
    public void AddLevel(int amount = 1) => Level += amount;
    public IEnumerable<StatModifier> GetModifiers() => _modifiers;
}