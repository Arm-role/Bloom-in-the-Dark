using System;

public class PlayerInventory
{
    private InventorySlot inventorySlotCache;

    public InventoryLogic Hotbar { get; private set; }
    public InventoryLogic MainInventory { get; private set; }

    public HotbarState HotbarState { get; private set; }

    public event Action<InventorySlot> OnHotbarSlotSelected;
    public PlayerInventory(HotbarState hotbarState, int hotbarSize, int inventorySize)
    {
        HotbarState = hotbarState;

        Hotbar = new InventoryLogic(hotbarSize);
        MainInventory = new InventoryLogic(inventorySize);
    }

    public InventorySlot GetHotbarSlotSelected()
    {
        if (inventorySlotCache == null)
        {
            inventorySlotCache = Hotbar.Slots[HotbarState.CurrentSlotIndex];
        }
        else if (inventorySlotCache != Hotbar.Slots[HotbarState.CurrentSlotIndex])
        {
            inventorySlotCache = Hotbar.Slots[HotbarState.CurrentSlotIndex];
        }

        return inventorySlotCache;
    }

    public int AddItem(IItemInstance item, int amount)
    {
        int remaining = amount;

        bool hotbarHasSameItem = Hotbar.Slots.Exists(
            s => !s.IsEmpty && s.Item.Data == item.Data && !s.IsFull);

        if (hotbarHasSameItem)
        {
            remaining = Hotbar.TryAddItem(item, remaining);
            if (remaining <= 0) return 0;
        }

        bool mainHasSameItem = MainInventory.Slots.Exists(
            s => !s.IsEmpty && s.Item.Data == item.Data && !s.IsFull);

        if (mainHasSameItem)
        {
            remaining = MainInventory.TryAddItem(item, remaining);
            if (remaining <= 0) return 0;
        }

        if (Hotbar.CanAddItem(item, remaining))
        {
            remaining = Hotbar.TryAddItem(item, remaining);
            if (remaining <= 0) return 0;
        }

        remaining = MainInventory.TryAddItem(item, remaining);

        return remaining;
    }

    public void MoveFromInventoryToHotbar(int inventorySlotIndex, int hotbarSlotIndex)
    {
        InventorySlot sourceSlot = MainInventory.Slots[inventorySlotIndex];
        InventorySlot destinationSlot = Hotbar.Slots[hotbarSlotIndex];

        if (!sourceSlot.IsEmpty && destinationSlot.IsEmpty)
        {
            destinationSlot.SetItem(sourceSlot.Item, sourceSlot.Amount);
            sourceSlot.Clear();
        }
    }
}
