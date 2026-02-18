using System;

public static class ItemFactory
{
    public static IItemInstance Create(IItemData data)
    {
        return data.Category switch
        {
            EItemCategory.Plant => new PlantItemInstance(data),
            _ => new ItemInstanceBase(data),
        };
    }
}