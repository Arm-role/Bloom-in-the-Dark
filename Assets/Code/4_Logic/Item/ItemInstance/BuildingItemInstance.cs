public class BuildingItemInstance : IItemInstance
{
    public IItemData ItemData { get; private set; }
    public float BuildingCout { get; private set; }
    public float EnergyReduceEachAction { get; private set; }

    public BuildingItemInstance(IItemData itemData)
    {
        ItemData = itemData;

        if (itemData != null && ItemData is IBuildItemData build)
        {
            BuildingCout = build.BuildingCout;
            EnergyReduceEachAction = build.EnergyReduceEachAction;
        }
    }
}
