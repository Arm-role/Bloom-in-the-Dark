public class InventoryPickContext
{
    public IItemInstance Item;
    public int Amount;
    public InventorySide SourceSide;
    public int SourceIndex;
    public bool IsHolding;

    public void Clear()
    {
        IsHolding = false;
        Item = null;
        Amount = 0;
    }
}