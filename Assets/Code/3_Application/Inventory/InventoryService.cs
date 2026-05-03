using System;
using System.Collections.Generic;

public sealed class InventoryService
{
  private readonly PlayerInventory _inventory;

  private readonly InventoryPickContext _pickContext = new();
  private HashSet<(InventorySide, int)> _sweepedSlots = new();

  public event Action OnInventoryChanged;
  public InventoryService(PlayerInventory inventory)
  {
    _inventory = inventory;
  }

  // =============================
  // Public Queries
  // =============================

  public IReadOnlyList<InventorySlot> GetHotbarSlots()
      => _inventory.GetHotbarSlots();

  public IReadOnlyList<InventorySlot> GetMainSlots()
      => _inventory.GetMainSlots();

  // ==============================
  // Click
  // ==============================

  public InventoryPickContext HandleClick(
      InventorySide side,
      int index,
      InventoryClickContext context)
  {
    // Shift → Quick Move
    if (context.IsShift)
    {
      if (_inventory.QuickMove(side, index))
        OnInventoryChanged?.Invoke();

      return _pickContext;
    }

    // Holding → Place
    if (_pickContext.IsHolding)
    {
      _inventory.Place(
          side,
          index,
          _pickContext.Item,
          _pickContext.Amount,
          _pickContext.SourceSide,
          _pickContext.SourceIndex);

      EndPick();
      return _pickContext;
    }

    // Not holding → Pick
    if (_inventory.TryPick(
        side,
        index,
        out var item,
        out var amount))
    {
      _pickContext.IsHolding = true;
      _pickContext.SourceSide = side;
      _pickContext.SourceIndex = index;
      _pickContext.Item = item;
      _pickContext.Amount = amount;

      OnInventoryChanged?.Invoke();
    }

    return _pickContext;
  }

  // ==============================
  // Drag Sweep (Shift + Hold)
  // ==============================

  public void HandleDragOver(
      InventorySide side,
      int index,
      bool isShift,
      bool isMouseDown)
  {
    if (!isShift || !isMouseDown)
    {
      _sweepedSlots.Clear();
      return;
    }

    if (_pickContext.IsHolding)
      return;

    if (_sweepedSlots.Contains((side, index)))
      return;

    if (_inventory.QuickMove(side, index))
    {
      _sweepedSlots.Add((side, index));
      OnInventoryChanged?.Invoke();
    }
  }

  // ==============================
  // Internal
  // ==============================

  private void EndPick()
  {
    _pickContext.Clear();
    OnInventoryChanged?.Invoke();
  }
}
