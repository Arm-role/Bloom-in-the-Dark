public struct InventoryDragContext
{
    public bool IsDragging;

    public InventorySide SourceSide;
    public int SourceIndex;

    public InventorySide TargetSide;
    public int TargetIndex;

    public InventorySlot SourceSlot;

    public void Clear()
    {
        IsDragging = false;
        SourceIndex = -1;
        TargetIndex = -1;
        SourceSlot = null;
    }
}
