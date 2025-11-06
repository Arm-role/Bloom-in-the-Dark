public class SeedItemInstance : IItemInstance
{
    public IItemData ItemData { get; private set; }
    public float EnergyReduceEachAction { get; private set; }

    public SeedItemInstance(IItemData itemData)
    {
        ItemData = itemData;

        if (itemData != null && ItemData is IEnergyReduce seed)
        {
            EnergyReduceEachAction = seed.EnergyReduceEachAction;
        }
    }
}