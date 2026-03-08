using UnityEngine;

public class ItemStack
{
    public IItemData ItemData { get; }
    public int Count { get; private set; }
    public int MaxStack => ItemData.MaxStackSize;

    public IItemInstance Instance { get; private set; }

    public ItemStack(IItemData itemData, int count = 1, IItemInstance instance = null)
    {
        ItemData = itemData;
        Count = count;
        Instance = instance;
    }

    public bool IsFull => Count >= MaxStack;

    public int Add(int amount)
    {
        int space = MaxStack - Count;
        int toAdd = Mathf.Min(space, amount);

        Count += toAdd;
        return amount - toAdd; // remainder
    }

    public int Remove(int amount)
    {
        int toRemove = Mathf.Min(amount, Count);
        Count -= toRemove;
        return toRemove;
    }
}