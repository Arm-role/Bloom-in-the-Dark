public class PlacementAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();

        bool canRemoveItem = false;
        ValidationResult canInteraction = new();
        bool activeAction = false;
        IDataProvider dataProvider = null;

        result.TargetDetector = (handle) =>
        {
            dataProvider = handle.IntercationExcute(context);
        };

        result.TargetValidator = (validator) =>
        {
            if (dataProvider.IsValid)
            {
                canInteraction = validator.Validate(dataProvider);
            }
        };

        result.InventoryInteraction = (inventory) =>
        {
            canRemoveItem = inventory.CanRemoveItem(context.ItemInstance.Data, 1);
            if (canInteraction.IsValid && canRemoveItem)
            {
                int remaining = inventory.TryRemoveItem(context.ItemInstance.Data, 1);
            }
        };

        if (canInteraction.IsValid && canRemoveItem)
        {
            result.PlayerData = (playerData) =>
            {
                var dir = (dataProvider.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            };

            result.ActionPerformer = async (action) =>
            {
                activeAction = await action.Execute(context, dataProvider);
            };
        }

        return result;
    }

    public ActionExecutionResult SecondaryActionExecute(InteractionHandleContext context)
    {
        throw new System.NotImplementedException();
    }
}
