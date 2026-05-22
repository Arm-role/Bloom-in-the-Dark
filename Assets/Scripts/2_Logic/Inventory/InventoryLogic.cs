using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryLogic : IInventoryLogic
{
    private readonly List<InventorySlot> _slots;
    public IReadOnlyList<InventorySlot> Slots => _slots;
    public int Capacity { get; }

    public event System.Action<IItemDefinition, int> OnItemAdded;
    public event System.Action<IItemDefinition, int> OnItemRemoved;

    public InventoryLogic(int capacity)
    {
        Capacity = capacity;
        _slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }
    public bool CanAddItem(IItemInstance item, int amount)
    {
        int remaining = amount;

        foreach (var slot in _slots)
        {
            if (!slot.IsEmpty && slot.GetItemInstance().Data == item.Data)
            {
                int space = slot.GetItemInstance().Data.MaxStackSize - slot.Amount;
                remaining -= space;
                if (remaining <= 0) return true;
            }
        }

        int emptySlots = _slots.Count(s => s.IsEmpty);
        int maxStack = item.Data.MaxStackSize;

        return remaining <= emptySlots * maxStack;
    }

    public int TryAddItem(IItemInstance item, int amount)
    {
        foreach (var slot in _slots.Where(s => !s.IsEmpty && !s.IsFull && s.GetItemInstance().Data == item.Data))
        {
            int spaceAvailable = slot.GetItemInstance().Data.MaxStackSize - slot.Amount;
            int amountToAdd = Mathf.Min(amount, spaceAvailable);

            slot.AddAmount(amountToAdd);
            amount -= amountToAdd;
            OnItemAdded?.Invoke(item.Data, amountToAdd);

            if (amount <= 0) return 0;
        }

        foreach (var slot in _slots.Where(s => s.IsEmpty))
        {
            int amountToAdd = Mathf.Min(amount, item.Data.MaxStackSize);

            slot.SetItem(item, amountToAdd);
            amount -= amountToAdd;
            OnItemAdded?.Invoke(item.Data, amountToAdd);

            if (amount <= 0) return 0;
        }

        return amount;
    }
    public void SwapSlots(int a, int b)
    {
        (_slots[a], _slots[b]) = (_slots[b], _slots[a]);
    }

    public bool CanRemoveItem(IItemDefinition itemData, int amount)
    {
        return CountItem(itemData) >= amount;
    }

    public int CountItem(IItemDefinition itemData)
    {
        return _slots
            .Where(s => !s.IsEmpty && s.GetItemInstance().Data == itemData)
            .Sum(s => s.Amount);
    }

    public int TryRemoveItem(IItemDefinition itemData, int amount)
    {
        int remaining = amount;

        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            var slot = _slots[i];
            if (!slot.IsEmpty && slot.GetItemInstance().Data == itemData)
            {
                int amountToRemove = Mathf.Min(remaining, slot.Amount);

                slot.RemoveAmount(amountToRemove);
                remaining -= amountToRemove;
                OnItemRemoved?.Invoke(itemData, amountToRemove);

                if (remaining <= 0)
                    return 0; 
            }
        }

        return remaining; 
    }
}
