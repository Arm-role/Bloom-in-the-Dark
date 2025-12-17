using System;

public static class ItemFactory
{
    public static IItemInstance Create(IItemData data)
    {
        return data.Type switch
        {
            EItemType.Tool => new ToolItemInstance(data),
            EItemType.Seed => new SeedItemInstance(data),
            EItemType.Plant => new PlantItemInstance(data),
            EItemType.Building => new BuildingItemInstance(data),
            _ => throw new Exception("Unknown item type")
        };
    }
}