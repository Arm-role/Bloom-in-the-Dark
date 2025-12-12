using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject, IItemData
{
    [Header("ItemData")]
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private PriorityRuleSO priorityRule;

    public int ID => itemId;
    public string Name => itemName;
    public Sprite Icon => itemIcon;
    public IPriorityRule PriorityRule => priorityRule;

    public abstract EItemType Type { get; }
    public abstract IReadOnlyList<InputStrategyBinding> StrategyBindings { get; }
    public abstract int MaxStackSize { get; }

    private void OnValidate()
    {
        itemName = name;
    }

    public EItemStrategyType ResolveStrategy(InputActionType input)
    {
        foreach (var bind in StrategyBindings)
        {
            if (input.HasFlag(bind.Input))
                return bind.Strategy;
        }

        return EItemStrategyType.None;
    }
}
