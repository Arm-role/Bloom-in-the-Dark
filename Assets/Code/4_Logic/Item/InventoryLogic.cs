using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryLogic
{
    public readonly List<InventorySlot> Slots;
    public readonly int Capacity;

    public event System.Action<IItemData, int> OnItemAdded;
    public event System.Action<IItemData, int> OnItemRemoved;

    public InventoryLogic(int capacity)
    {
        Capacity = capacity;
        Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    public int TryAddItem(IItemInstance item, int amount)
    {
        foreach (var slot in Slots.Where(s => !s.IsEmpty && !s.IsFull && s.Item.ItemData == item.ItemData))
        {
            int spaceAvailable = slot.Item.ItemData.MaxStackSize - slot.Amount;
            int amountToAdd = Mathf.Min(amount, spaceAvailable);

            slot.AddAmount(amountToAdd);
            amount -= amountToAdd;
            OnItemAdded?.Invoke(item.ItemData, amountToAdd);

            if (amount <= 0) return 0; 
        }

        foreach (var slot in Slots.Where(s => s.IsEmpty))
        {
            int amountToAdd = Mathf.Min(amount, item.ItemData.MaxStackSize);

            slot.SetItem(item, amountToAdd);
            amount -= amountToAdd;
            OnItemAdded?.Invoke(item.ItemData, amountToAdd);

            if (amount <= 0) return 0; 
        }

        return amount;
    }

    public void RemoveItem(IItemData itemData, int amount)
    {
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            var slot = Slots[i];
            if (!slot.IsEmpty && slot.Item.ItemData == itemData)
            {
                int amountToRemove = Mathf.Min(amount, slot.Amount);

                slot.RemoveAmount(amountToRemove);
                amount -= amountToRemove;
                OnItemRemoved?.Invoke(itemData, amountToRemove);

                if (amount <= 0) return; 
            }
        }
    }
}
