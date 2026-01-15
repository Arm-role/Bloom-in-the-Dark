using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject, IItemData
{
    [Header("ItemData")] 
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private ItemInteractionProfile _interactionProfile;

    [Header("Stats")]
    [SerializeField] private List<ItemStatEntry> baseStats;
    [SerializeField] private List<ItemStatEntry> perLevelStats;
    
    [Header("Properties")]
    [SerializeField] private List<ItemPropertyEntry> properties;

    private Dictionary<EItemStatType, float> _baseStatMap;
    private Dictionary<EItemStatType, float> _perLevelStatMap;
    private Dictionary<EItemProperty, ItemPropertyEntry> _propertyMap;
    
    protected virtual void OnEnable()
    {
        _baseStatMap = BuildStatMap(baseStats);
        _perLevelStatMap = BuildStatMap(perLevelStats);
        _propertyMap = BuildPropertyMap(properties);
    }
    
    public bool SupportsStat(EItemStatType stat)
        => _baseStatMap.ContainsKey(stat);

    public float GetBaseStat(EItemStatType stat)
        => _baseStatMap.TryGetValue(stat, out var v) ? v : 0f;

    public float GetPerLevelStat(EItemStatType stat)
        => _perLevelStatMap.TryGetValue(stat, out var v) ? v : 0f;

    public bool SupportsProperty(EItemProperty prop)
        => _propertyMap.ContainsKey(prop);

    public T GetProperty<T>(EItemProperty property)
    {
        var entry = _propertyMap[property];

        object value = typeof(T) switch
        {
            var t when t == typeof(int) => entry.IntValue,
            var t when t == typeof(float) => entry.FloatValue,
            var t when t == typeof(string) => entry.StringValue,
            var t when t == typeof(Vector2Int) => entry.Vector2IntValue,
            _ => null
        };

        return (T)value;
    }
    
    public int ID => itemId;
    public string Name => itemName;
    public Sprite Icon => itemIcon;
    public IItemInteractionProfile InteractionProfile => _interactionProfile;

    public abstract EItemType Type { get; }
    public abstract int MaxStackSize { get; }

    // ---------- Helpers ----------

    private static Dictionary<EItemStatType, float> BuildStatMap(
        List<ItemStatEntry> list)
    {
        var dict = new Dictionary<EItemStatType, float>();
        foreach (var e in list)
            dict[e.Stat] = e.Value;
        return dict;
    }

    private static Dictionary<EItemProperty, ItemPropertyEntry> BuildPropertyMap(
        List<ItemPropertyEntry> list)
    {
        var dict = new Dictionary<EItemProperty, ItemPropertyEntry>();
        foreach (var e in list)
            dict[e.Property] = e;
        return dict;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        itemName = name;
    }
#endif
}