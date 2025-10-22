public class ToolItemInstance : IItemInstance
{
    public IItemData ItemData { get; private set; }
    public float EnergyReduce { get; private set; }

    public ToolItemInstance(IItemData itemData)
    {
        ItemData = itemData;
        
        if(itemData != null && ItemData is IToolItemData tool)
        {
            EnergyReduce = tool.EnergyReduce;
        }
    }
}
