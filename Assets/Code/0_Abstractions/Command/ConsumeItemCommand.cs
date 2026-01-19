public readonly struct ConsumeItemCommand : IPlayerCommand
{
    public readonly IItemData ItemData;
    public readonly int Amount;

    public ConsumeItemCommand(IItemData itemData, int amount)
    {
        ItemData = itemData;    
        Amount = amount;
    }
}