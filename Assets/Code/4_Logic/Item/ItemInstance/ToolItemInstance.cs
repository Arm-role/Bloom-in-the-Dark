public class ToolItemInstance : IItemInstance
{
    public IItemData ItemData { get; private set; }
    public float EnergyReduceEachAction { get; private set; }


    public ToolItemInstance(IItemData itemData)
    {
        ItemData = itemData;
        
        if(itemData != null && ItemData is IToolItemData tool)
        {
            EnergyReduceEachAction = tool.EnergyReduceEachAction;
        }
    }
}
