public sealed class InventoryPickContext
{
  public bool IsHolding;
  public InventorySide SourceSide;
  public int SourceIndex;

  public IItemInstance Item;
  public int Amount;

  public void Clear()
  {
    IsHolding = false;
    Item = null;
    Amount = 0;
    SourceIndex = -1;
  }
}