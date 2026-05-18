using System;
using UnityEngine;

public class InventorySlot
{
  private IItemInstance _item;
  public int Amount { get; private set; }
  public int ItemId { get; private set; }
  public string DisplayName { get; private set; }

  public bool IsEmpty => _item == null || Amount <= 0;
  public bool IsFull => !IsEmpty && Amount >= _item.Data.MaxStackSize;



  public event Action<InventorySlot> OnSlotChanged;

  public void SetItem(IItemInstance item, int amount)
  {
    if (item == null || amount <= 0)
    {
      Clear();
      return;
    }

    _item = item;
    Amount = Mathf.Min(amount, item.Data.MaxStackSize);
    ItemId = item.Data.ID;
    DisplayName = item.Data.Name;
    OnSlotChanged?.Invoke(this);
  }

  public void AddAmount(int amount)
  {
    if (IsEmpty) return;

    Amount = Mathf.Min(Amount + amount, _item.Data.MaxStackSize);
    OnSlotChanged?.Invoke(this);
  }

  public void RemoveAmount(int amount)
  {
    if (IsEmpty) return;

    Amount -= amount;
    if (Amount <= 0) Clear();
    else OnSlotChanged?.Invoke(this);
  }

  public void Clear()
  {
    _item = null;
    Amount = 0;
    OnSlotChanged?.Invoke(this);
  }

  // =============================
  // Helper
  // =============================

  public IItemInstance GetItemInstance() => _item;
}