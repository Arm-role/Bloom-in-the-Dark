using System;
using System.Collections.Generic;

public interface IInventoryView
{
    void CreateSlots(int capacity);

    void UpdateAllSlots(List<SlotDisplayData> slotData);

    void UpdateSlot(int index, SlotDisplayData slotData);

    void SelectSlot(int slotIndex);

    event Action<int> OnSlotClicked;
}