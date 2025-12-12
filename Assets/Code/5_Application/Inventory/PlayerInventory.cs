using System;

public class PlayerInventory
{
    private InventorySlot inventorySlotCache;

    public InventoryLogic Hotbar { get; }
    public InventoryLogic MainInventory { get; }
    public HotbarState HotbarState { get; }

    public event Action<InventorySlot> OnHotbarSlotSelected;

    public PlayerInventory(HotbarState hotbarState, int hotbarSize, int inventorySize)
    {
        HotbarState = hotbarState;
        Hotbar = new InventoryLogic(hotbarSize);
        MainInventory = new InventoryLogic(inventorySize);
    }

    // ---------------------------------------------------------
    // 🔹 GET CURRENT SELECTED HOTBAR SLOT (CACHED)
    // ---------------------------------------------------------
    public InventorySlot GetHotbarSlotSelected()
    {
        int index = HotbarState.CurrentSlotIndex;

        if (inventorySlotCache != Hotbar.Slots[index])
            inventorySlotCache = Hotbar.Slots[index];

        return inventorySlotCache;
    }

    // ---------------------------------------------------------
    // 🔹 ADD ITEM (Smart + Clean)
    // ---------------------------------------------------------
    public int AddItem(IItemInstance item, int amount)
    {
        int remaining = amount;

        // 1) Fill existing stacks first (Hotbar → Main)
        remaining = Hotbar.TryAddItem(item, remaining);
        remaining = MainInventory.TryAddItem(item, remaining);

        if (remaining <= 0)
            return 0;

        // 2) Fill empty slots (Hotbar → Main)
        if (Hotbar.CanAddItem(item, remaining))
            remaining = Hotbar.TryAddItem(item, remaining);

        return MainInventory.TryAddItem(item, remaining);
    }

    // ---------------------------------------------------------
    // 🔹 MOVE / SWAP SLOTS
    // ---------------------------------------------------------

    public void SwapHotbarSlots(int a, int b)
    {
        (Hotbar.Slots[a], Hotbar.Slots[b]) = (Hotbar.Slots[b], Hotbar.Slots[a]);
    }

    public void SwapMainSlots(int a, int b)
    {
        (MainInventory.Slots[a], MainInventory.Slots[b]) =
        (MainInventory.Slots[b], MainInventory.Slots[a]);
    }

    // ---------------------------------------------------------
    // 🔹 MOVE ITEM Inventory → Hotbar
    // ---------------------------------------------------------
    public bool MoveFromInventoryToHotbar(int inventorySlotIndex, int hotbarSlotIndex)
    {
        if (!IsValidIndex(MainInventory, inventorySlotIndex)) return false;
        if (!IsValidIndex(Hotbar, hotbarSlotIndex)) return false;

        var src = MainInventory.Slots[inventorySlotIndex];
        var dst = Hotbar.Slots[hotbarSlotIndex];

        if (!src.IsEmpty && dst.IsEmpty)
        {
            dst.SetItem(src.Item, src.Amount);
            src.Clear();
            return true;
        }

        return false;
    }

    // ---------------------------------------------------------
    // 🔹 MOVE Hotbar → Inventory (ใช้ Swap ร่วม)
    // ---------------------------------------------------------
    public bool MoveHotbarToInventory(int hotbarIndex, int inventoryIndex)
    {
        if (!IsValidIndex(Hotbar, hotbarIndex)) return false;
        if (!IsValidIndex(MainInventory, inventoryIndex)) return false;

        var src = Hotbar.Slots[hotbarIndex];
        var dst = MainInventory.Slots[inventoryIndex];

        if (!src.IsEmpty && dst.IsEmpty)
        {
            dst.SetItem(src.Item, src.Amount);
            src.Clear();
            return true;
        }

        return false;
    }

    // ---------------------------------------------------------
    // 🔹 VALID INDEX CHECKER
    // ---------------------------------------------------------
    private bool IsValidIndex(InventoryLogic inv, int index) =>
        index >= 0 && index < inv.Capacity;
}