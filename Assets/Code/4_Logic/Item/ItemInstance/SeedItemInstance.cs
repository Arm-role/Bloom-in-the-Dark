public class SeedItemInstance : IItemInstance
{
    public IItemData Data { get; private set; }
    public SeedItemInstance(IItemData data)
    {
        Data = data;
    }
}