using System;
using System.Collections.Generic;

public class PlayerInventory
{
  private InventorySlot _inventorySlotCache;
  private IItemInstance _emptyItem;

  public InventoryLogic Hotbar { get; }
  public InventoryLogic MainInventory { get; }
  public HotbarState HotbarState { get; }

  public event Action<InventorySlot> OnHotbarSlotSelected;

  public PlayerInventory(
    IItemInstance emptyItem,
    HotbarState hotbarState,
    InventoryLogic hotbarLogic,
    InventoryLogic inventoryLogic)
  {
    _emptyItem = emptyItem;

    HotbarState = hotbarState;
    Hotbar = hotbarLogic;
    MainInventory = inventoryLogic;
  }

  // ==============================
  // Queries
  // ==============================

  public IReadOnlyList<InventorySlot> GetHotbarSlots()
      => Hotbar.Slots;

  public IReadOnlyList<InventorySlot> GetMainSlots()
      => MainInventory.Slots;

  public InventorySlot GetSlot(InventorySide side, int index)
      => GetInventory(side).Slots[index];

  // ---------------------------------------------------------
  // 🔹 GET CURRENT SELECTED HOTBAR SLOT (CACHED)
  // ---------------------------------------------------------
  public InventorySlot GetHotbarSlotSelected()
  {
    int index = HotbarState.CurrentSlotIndex;

    if (_inventorySlotCache != Hotbar.Slots[index])
      _inventorySlotCache = Hotbar.Slots[index];

    return _inventorySlotCache;
  }

  public IItemInstance GetEmptyItem()
  => _emptyItem;

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

  public bool CanRemoveItem(IItemDefinition itemData, int amount)
  {
    return Hotbar.CanRemoveItem(itemData, amount);
  }

  public int TryRemoveItem(IItemDefinition itemData, int amount)
  {
    return Hotbar.TryRemoveItem(itemData, amount);
  }

  // ==============================
  // Pick / Place / Swap (Domain)
  // ==============================

  public bool TryPick(InventorySide side, int index,
      out IItemInstance item,
      out int amount)
  {
    item = null;
    amount = 0;

    var slot = GetSlot(side, index);

    if (slot.IsEmpty)
      return false;

    item = slot.GetItemInstance();
    amount = slot.Amount;

    slot.Clear();
    return true;
  }

  public void Place(
      InventorySide side,
      int index,
      IItemInstance item,
      int amount,
      InventorySide sourceSide,
      int sourceIndex)
  {
    var targetSlot = GetSlot(side, index);

    // วางกลับช่องเดิม
    if (side == sourceSide && index == sourceIndex)
    {
      targetSlot.SetItem(item, amount);
      return;
    }

    if (targetSlot.IsEmpty)
    {
      targetSlot.SetItem(item, amount);
    }
    else
    {
      // Swap
      var tempItem = targetSlot.GetItemInstance();
      var tempAmount = targetSlot.Amount;

      targetSlot.SetItem(item, amount);

      var sourceSlot = GetSlot(sourceSide, sourceIndex);
      sourceSlot.SetItem(tempItem, tempAmount);
    }
  }

  // ---------------------------------------------------------
  // 🔹 MOVE / SWAP SLOTS
  // ---------------------------------------------------------
  public bool QuickMove(InventorySide side, int index)
  {
    var from = GetInventory(side);
    var to = GetOppositeInventory(side);

    var source = from.Slots[index];
    if (source.IsEmpty)
      return false;

    int targetIndex = FindFirstEmptySlot(to);
    if (targetIndex < 0)
      return false;

    to.Slots[targetIndex]
        .SetItem(source.GetItemInstance(), source.Amount);

    source.Clear();

    return true;
  }

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
      dst.SetItem(src.GetItemInstance(), src.Amount);
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
      dst.SetItem(src.GetItemInstance(), src.Amount);
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

  // =============================
  // Helpers
  // =============================

  private InventoryLogic GetInventory(InventorySide side)
      => side == InventorySide.Hotbar
          ? Hotbar
          : MainInventory;

  private InventoryLogic GetOppositeInventory(
      InventorySide side)
      => side == InventorySide.Hotbar
          ? MainInventory
          : Hotbar;

  private int FindFirstEmptySlot(
      InventoryLogic inventory)
  {
    for (int i = 0; i < inventory.Slots.Count; i++)
    {
      if (inventory.Slots[i].IsEmpty)
        return i;
    }

    return -1;
  }
}
