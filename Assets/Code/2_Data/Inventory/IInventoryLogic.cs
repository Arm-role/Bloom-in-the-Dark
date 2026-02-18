public interface IInventoryLogic
{
    int TryAddItem(IItemInstance item, int amount);
    bool CanRemoveItem(IItemData itemData, int amount);
    int TryRemoveItem(IItemData itemData, int amount);
}