#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class InventoryService
{
  private readonly PlayerInventory _inventory;
  private readonly IAudioService? _audio;
  private readonly IInventorySoundConfig? _soundConfig;

  private readonly InventoryPickContext _pickContext = new();
  private readonly HashSet<(InventorySide, int)> _sweepedSlots = new();

  public event Action? OnInventoryChanged;

  public InventoryService(
      PlayerInventory inventory,
      IAudioService? audio = null,
      IInventorySoundConfig? soundConfig = null)
  {
    _inventory = inventory;
    _audio = audio;
    _soundConfig = soundConfig;
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
      {
        PlaySfx(_soundConfig?.OnQuickMove);
        OnInventoryChanged?.Invoke();
      }
      else
      {
        PlaySfx(_soundConfig?.OnFail);
      }

      return _pickContext;
    }

    // Holding → Place
    if (_pickContext.IsHolding && _pickContext.Item != null)
    {
      var result = _inventory.Place(
          side,
          index,
          _pickContext.Item,
          _pickContext.Amount,
          _pickContext.SourceSide,
          _pickContext.SourceIndex);

      PlaySfx(PickPlaceSfx(result));
      EndPick();
      return _pickContext;
    }

    // Not holding → Pick
    if (_inventory.TryPick(
        side,
        index,
        out var item,
        out var amount) && item != null)
    {
      _pickContext.IsHolding = true;
      _pickContext.SourceSide = side;
      _pickContext.SourceIndex = index;
      _pickContext.Item = item;
      _pickContext.Amount = amount;

      PlaySfx(_soundConfig?.OnPick);
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
      PlaySfx(_soundConfig?.OnQuickMove);
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

  private void PlaySfx(SoundKey? key)
  {
    if (_audio == null || key == null) return;
    _audio.PlaySFX(key, Vector3.zero);
  }

  private SoundKey? PickPlaceSfx(PlaceResult result) => result switch
  {
    PlaceResult.Swapped => _soundConfig?.OnSwap,
    PlaceResult.ReturnedToSource => _soundConfig?.OnFail ?? _soundConfig?.OnPlace,
    _ => _soundConfig?.OnPlace,
  };
}
