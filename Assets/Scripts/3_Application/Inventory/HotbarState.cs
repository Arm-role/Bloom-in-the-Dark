
using System;

public class HotbarState
{
    public int CurrentSlotIndex { get; private set; }
    private readonly int _totalSlots;

    public event Action<int> OnSlotChanged;

    public HotbarState(int totalSlots)
    {
        _totalSlots = totalSlots;
        CurrentSlotIndex = 0;
    }

    public void SelectNextSlot()
    {
        CurrentSlotIndex++;
        if (CurrentSlotIndex >= _totalSlots)
        {
            CurrentSlotIndex = 0;
        }
        OnSlotChanged?.Invoke(CurrentSlotIndex);
    }
    public void SelectPreviousSlot()
    {
        CurrentSlotIndex--;
        if (CurrentSlotIndex < 0)
        {
            CurrentSlotIndex = _totalSlots - 1;
        }
        OnSlotChanged?.Invoke(CurrentSlotIndex);
    }
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= _totalSlots) return;

        CurrentSlotIndex = index;
        OnSlotChanged?.Invoke(CurrentSlotIndex);
    }
}