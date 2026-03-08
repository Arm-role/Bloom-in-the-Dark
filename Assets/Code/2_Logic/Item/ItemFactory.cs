public static class ItemFactory
{
    public static IItemInstance Create(IItemData data)
    {
        return data.Category switch
        {
            _ => new ItemInstanceBase(data),
        };
    }
}