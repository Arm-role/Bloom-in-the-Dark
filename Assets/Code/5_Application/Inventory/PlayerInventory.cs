public class PlayerInventory
{
    public InventoryLogic Hotbar { get; private set; }
    public InventoryLogic MainInventory { get; private set; }
    public HotbarState HotbarState { get; private set; }
    public PlayerInventory(HotbarState hotbarState, int hotbarSize, int inventorySize)
    {
        HotbarState = hotbarState;

        Hotbar = new InventoryLogic(hotbarSize);
        MainInventory = new InventoryLogic(inventorySize);
    }

    public InventorySlot GetHotbarSlotSelected()
    {
        return Hotbar.Slots[HotbarState.CurrentSlotIndex];
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
