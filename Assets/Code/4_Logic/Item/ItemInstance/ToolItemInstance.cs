public class ToolItemInstance : IItemInstance
{
    public IItemData Data { get; private set; }

    public ToolItemInstance(IItemData itemData)
    {
        Data = itemData;
    }
}
