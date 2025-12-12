using System.Collections.Generic;
using UnityEngine;

public interface IItemData
{
    int ID { get; }
    string Name { get; }
    EItemType Type { get; }
    IReadOnlyList<InputStrategyBinding> StrategyBindings { get; }

    Sprite Icon { get; }
    IPriorityRule PriorityRule { get; }
    int MaxStackSize { get; }
    EItemStrategyType ResolveStrategy(InputActionType input);
}
