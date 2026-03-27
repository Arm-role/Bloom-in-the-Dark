public interface IInventoryLogic
{
    int TryAddItem(IItemInstance item, int amount);
    bool CanRemoveItem(IItemDefinition itemData, int amount);
    int TryRemoveItem(IItemDefinition itemData, int amount);
}