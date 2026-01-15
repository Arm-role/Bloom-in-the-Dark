using System;
using UnityEngine;

public class InventorySlot
{
    public IItemInstance Item { get; private set; }
    public int Amount { get; private set; }

    public bool IsEmpty => Item == null || Amount <= 0;
    public bool IsFull => !IsEmpty && Amount >= Item.Data.MaxStackSize;

    public event Action<InventorySlot> OnSlotChanged;

    public void SetItem(IItemInstance item, int amount)
    {
        if (item == null || amount <= 0)
        {
            Clear();
            return;
        }

        Item = item;
        Amount = Mathf.Min(amount, item.Data.MaxStackSize);
        OnSlotChanged?.Invoke(this);
    }

    public void AddAmount(int amount)
    {
        if (IsEmpty) return;

        Amount = Mathf.Min(Amount + amount, Item.Data.MaxStackSize);
        OnSlotChanged?.Invoke(this);
    }

    public void RemoveAmount(int amount)
    {
        if (IsEmpty) return;

        Amount -= amount;
        if (Amount <= 0) Clear();
        else OnSlotChanged?.Invoke(this);
    }

    public void Clear()
    {
        Item = null;
        Amount = 0;
        OnSlotChanged?.Invoke(this);
    }
}