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
