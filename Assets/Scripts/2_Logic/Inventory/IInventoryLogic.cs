using System.Collections.Generic;

public interface IInventoryLogic
{
    IReadOnlyList<InventorySlot> Slots { get; }
    int Capacity { get; }
    bool CanAddItem(IItemInstance item, int amount);
    int TryAddItem(IItemInstance item, int amount);
    bool CanRemoveItem(IItemDefinition itemData, int amount);
    int TryRemoveItem(IItemDefinition itemData, int amount);
    void SwapSlots(int a, int b);
}