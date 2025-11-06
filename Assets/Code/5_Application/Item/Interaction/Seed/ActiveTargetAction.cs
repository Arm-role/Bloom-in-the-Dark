
public class ActiveTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();
        var process = new ProcessState<IDataProvider, bool>();

        result.TargetDetector = (handle) =>
        {
            process.Source = handle.IntercationExcute(context);
        };

        result.InventoryInteraction = (inventory) =>
        {
            bool canRemove = inventory.CanRemoveItem(context.ItemInstance.ItemData, 1);
            if (process.Source != null && canRemove)
            {
                if (process.Source is DirectInteractData data)
                {
                    process.Target = data.Target.IsVaild;
                    int remaining = inventory.TryRemoveItem(context.ItemInstance.ItemData, 1);
                }
            }
        };

        result.PlayerData = (playerData) =>
        {
            if (process.Source != null && process.Target)
            {
                var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            }
        };

        result.ActionPerformer = (action) =>
        {
            if (process.Source != null && process.Target)
            {
                action.Execute(process.Source);
            }
        };

        return result;
    }
}

