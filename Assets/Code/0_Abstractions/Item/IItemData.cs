using UnityEngine;

public interface IItemData
{
    int ID { get; }
    string Name { get; }
    EItemType Type { get; }
    EItemStategyType StategyType { get; }

    Sprite Icon { get; }

    int MaxStackSize { get; }
}
