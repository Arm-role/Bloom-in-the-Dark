#nullable enable

using System;
using System.Collections.Generic;

public sealed class PlayerInventory
{
  private readonly IItemInstance _emptyItem;

  public InventoryLogic Hotbar { get; }
  public InventoryLogic MainInventory { get; }
  public HotbarState HotbarState { get; }

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

  public InventorySlot GetHotbarSlotSelected()
      => Hotbar.Slots[HotbarState.CurrentSlotIndex];

  public IItemInstance GetEmptyItem()
    => _emptyItem;

  // ---------------------------------------------------------
  // 🔹 ADD ITEM
  // ---------------------------------------------------------
  // InventoryLogic.TryAddItem fill ทั้ง existing stack + empty slot ใน 1 call
  // → 1 รอบต่อ container ก็ครบ ไม่ต้องวน 2 pass
  public int AddItem(IItemInstance item, int amount)
  {
    int remaining = Hotbar.TryAddItem(item, amount);
    if (remaining <= 0) return 0;

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

  // นับ/remove จากทั้ง Hotbar + Main — ใช้ตอน trade ที่ต้องหัก input จากทุกที่
  public int CountItemAnywhere(IItemDefinition itemData)
  {
    return Hotbar.CountItem(itemData) + MainInventory.CountItem(itemData);
  }

  public bool CanRemoveItemAnywhere(IItemDefinition itemData, int amount)
  {
    return CountItemAnywhere(itemData) >= amount;
  }

  // คืนค่า remaining ที่หักไม่ครบ (0 = หักครบ)
  public int RemoveItemAnywhere(IItemDefinition itemData, int amount)
  {
    int remaining = Hotbar.TryRemoveItem(itemData, amount);
    return MainInventory.TryRemoveItem(itemData, remaining);
  }

  // ==============================
  // Pick / Place / Swap (Domain)
  // ==============================

  public bool TryPick(
      InventorySide side,
      int index,
      out IItemInstance? item,
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

    var sourceSlot = GetSlot(sourceSide, sourceIndex);

    if (targetSlot.IsEmpty)
    {
      targetSlot.SetItem(item, amount);
      return;
    }

    var targetItem = targetSlot.GetItemInstance()!;

    // Same-type → merge, ส่วนเกินคืนช่องเดิม (source slot ว่างหลัง Pick)
    if (targetItem.Data == item.Data)
    {
      int space = item.Data.MaxStackSize - targetSlot.Amount;
      if (space > 0)
      {
        int merged = Math.Min(amount, space);
        targetSlot.AddAmount(merged);
        int overflow = amount - merged;
        if (overflow > 0)
          sourceSlot.SetItem(item, overflow);
        return;
      }

      // target เต็มแล้ว → คืนช่องเดิม (ไม่ swap ของ same-type)
      sourceSlot.SetItem(item, amount);
      return;
    }

    // Different-type → swap
    int tempAmount = targetSlot.Amount;
    targetSlot.SetItem(item, amount);
    sourceSlot.SetItem(targetItem, tempAmount);
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

    var item = source.GetItemInstance()!;
    int amount = source.Amount;

    // TryAddItem จัดการ fill-existing-stack + empty-slot ครบใน 1 call
    int leftover = to.TryAddItem(item, amount);
    int moved = amount - leftover;

    if (moved <= 0)
      return false;

    source.RemoveAmount(moved);
    return true;
  }

  public void SwapHotbarSlots(int a, int b) => Hotbar.SwapSlots(a, b);

  public void SwapMainSlots(int a, int b) => MainInventory.SwapSlots(a, b);

  // ---------------------------------------------------------
  // 🔹 MOVE ITEM Inventory → Hotbar
  // ---------------------------------------------------------
  public bool MoveFromInventoryToHotbar(int inventorySlotIndex, int hotbarSlotIndex)
      => MoveBetween(MainInventory, inventorySlotIndex, Hotbar, hotbarSlotIndex);

  // ---------------------------------------------------------
  // 🔹 MOVE Hotbar → Inventory
  // ---------------------------------------------------------
  public bool MoveHotbarToInventory(int hotbarIndex, int inventoryIndex)
      => MoveBetween(Hotbar, hotbarIndex, MainInventory, inventoryIndex);

  // Merge ถ้า dst เป็น same-type, swap ถ้า diff-type, ย้ายถ้า dst ว่าง
  private bool MoveBetween(InventoryLogic from, int fromIndex, InventoryLogic to, int toIndex)
  {
    if (!IsValidIndex(from, fromIndex)) return false;
    if (!IsValidIndex(to, toIndex)) return false;

    var src = from.Slots[fromIndex];
    if (src.IsEmpty) return false;

    var srcItem = src.GetItemInstance()!;
    int srcAmount = src.Amount;
    var dst = to.Slots[toIndex];

    if (dst.IsEmpty)
    {
      dst.SetItem(srcItem, srcAmount);
      src.Clear();
      return true;
    }

    var dstItem = dst.GetItemInstance()!;

    // Merge same-type
    if (dstItem.Data == srcItem.Data)
    {
      int space = srcItem.Data.MaxStackSize - dst.Amount;
      if (space <= 0) return false;

      int moved = Math.Min(srcAmount, space);
      dst.AddAmount(moved);
      src.RemoveAmount(moved);
      return true;
    }

    // Swap different-type
    int dstAmount = dst.Amount;
    dst.SetItem(srcItem, srcAmount);
    src.SetItem(dstItem, dstAmount);
    return true;
  }

  // ---------------------------------------------------------
  // 🔹 VALID INDEX CHECKER
  // ---------------------------------------------------------
  private static bool IsValidIndex(InventoryLogic inv, int index) =>
      index >= 0 && index < inv.Capacity;

  // =============================
  // Helpers
  // =============================

  private InventoryLogic GetInventory(InventorySide side)
      => side == InventorySide.Hotbar
          ? Hotbar
          : MainInventory;

  private InventoryLogic GetOppositeInventory(InventorySide side)
      => side == InventorySide.Hotbar
          ? MainInventory
          : Hotbar;
}
