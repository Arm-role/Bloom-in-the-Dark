using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    public GameObject Root { get; }

    event Action<int> OnSlotClicked;
    event Action<int> OnSlotHovered;
    event Action<int> OnSlotDraggedOver;
    public InventoryViewMode Mode { get; }

    void CreateSlots(int capacity);
    void SetMode(InventoryViewMode mode);
    void UpdateAllSlots(List<SlotDisplayData> slotData);

    void UpdateSlot(int index, SlotDisplayData slotData);

    void SelectSlot(int slotIndex);
    void UnselectAll();
}
