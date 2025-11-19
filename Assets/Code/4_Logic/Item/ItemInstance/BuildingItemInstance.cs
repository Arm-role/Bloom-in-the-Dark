public class BuildingItemInstance : IItemInstance
{
    public IItemData Data { get; private set; }
    public BuildingItemInstance(IItemData data)
    {
        Data = data;
    }
}
