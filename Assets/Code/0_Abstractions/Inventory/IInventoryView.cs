using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    public GameObject Root { get; }
    event Action<int> OnSlotClicked;
    event Action<int> OnSlotBeginDrag;
    event Action<int> OnSlotEndDrag;
    event Action<int> OnSlotDropOn;
    event Action<int> OnSlotDragEnter;
    void CreateSlots(int capacity);
    void SetMode(InventoryViewMode mode);
    void UpdateAllSlots(List<SlotDisplayData> slotData);

    void UpdateSlot(int index, SlotDisplayData slotData);

    void SelectSlot(int slotIndex);


    void UnselectAll();
}
