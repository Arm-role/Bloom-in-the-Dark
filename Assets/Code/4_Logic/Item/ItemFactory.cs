using System;

public static class ItemFactory
{
    public static IItemInstance Create(IItemData data)
    {
        return data.Type switch
        {
            EItemType.Plant => new PlantItemInstance(data),
            _ => new ItemInstanceBase(data),
        };
    }
}