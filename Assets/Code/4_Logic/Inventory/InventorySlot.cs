using System;
using UnityEngine;
public class InventorySlot
{
    public IItemInstance Item { get; private set; }
    public int Amount { get; private set; }

    public bool IsEmpty => Item == null || Amount <= 0;
    public bool IsFull => !IsEmpty && Amount >= Item.ItemData.MaxStackSize;

    public event Action<InventorySlot> OnSlotChanged;

    public InventorySlot()
    {
        Clear();
    }

    public void SetItem(IItemInstance item, int amount = 1)
    {
        if (item != null && amount > 0)
        {
            Item = item;
            Amount = Mathf.Min(amount, item.ItemData.MaxStackSize);
        }
        else
        {
            Clear();
        }
        OnSlotChanged?.Invoke(this);
    }

    public void AddAmount(int amount)
    {
        if (IsEmpty) return;
        Amount = Mathf.Min(Amount + amount, Item.ItemData.MaxStackSize);
        OnSlotChanged?.Invoke(this);
    }

    public void RemoveAmount(int amount)
    {
        if (IsEmpty) return;
        Amount -= amount;
        if (Amount <= 0)
        {
            Clear();
        }
        OnSlotChanged?.Invoke(this);
    }

    public void Clear()
    {
        Item = null;
        Amount = 0;
        OnSlotChanged?.Invoke(this);
    }
}
