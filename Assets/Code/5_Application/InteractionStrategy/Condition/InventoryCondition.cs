public class InventoryCondition : IActionCondition
{
    private readonly PlayerInventory _inv;
    private readonly IItemData _item;

    public InventoryCondition(PlayerInventory inv, IItemData item)
    {
        _inv = inv;
        _item = item;
    }

    public bool Check(IDataProvider data, InteractionHandleContext ctx, out string reason)
    {
        if (_inv.Hotbar.CanRemoveItem(_item, 1))
        {
            reason = null;
            return true;
        }

        reason = "No item in inventory";
        return false;
    }
}
