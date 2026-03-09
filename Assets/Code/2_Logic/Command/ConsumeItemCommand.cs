public readonly struct ConsumeItemCommand : IPlayerCommand
{
    public readonly IItemDefinition ItemData;
    public readonly int Amount;

    public ConsumeItemCommand(IItemDefinition itemData, int amount)
    {
        ItemData = itemData;    
        Amount = amount;
    }
}