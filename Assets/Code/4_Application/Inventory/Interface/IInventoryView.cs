using System;
using System.Collections.Generic;

public interface IInventoryView
{
  event Action<int> OnSlotClicked;
  event Action<int> OnSlotHovered;
  event Action<int> OnSlotDraggedOver;

  void CreateSlots(int capacity);
  void SetSlots(IReadOnlyList<SlotViewModel> slots);
  void Highlight(int index);
  void SetCooldown(int index, float normalized);
}
