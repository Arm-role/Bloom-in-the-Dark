
public class ActiveTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();
        var actionDetector = new ProcessState<IDataProvider, bool>();
        var inventoryDetector = new ProcessState<bool, bool>();

        result.TargetDetector = (detector) =>
        {
            actionDetector.Source = detector.IntercationExcute(context);
        };

        result.TargetValidator = (validator) =>
        {
            if (actionDetector.Source != null)
            {
                actionDetector.Target = validator.CanInteract(actionDetector.Source);
            }
        };

        result.InventoryInteraction = (inventory) =>
        {
            bool canRemove = inventory.CanRemoveItem(context.ItemInstance.ItemData, 1);
            if (actionDetector.Source != null && actionDetector.Target && canRemove)
            {
                if (actionDetector.Source is DirectInteractData data)
                {
                    inventoryDetector.Source = data.Target.IsValid;
                    int remaining = inventory.TryRemoveItem(context.ItemInstance.ItemData, 1);
                }
            }
        };

        result.PlayerData = (playerData) =>
        {
            if (actionDetector.Source != null && actionDetector.Target && inventoryDetector.Source)
            {
                var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            }
        };

        result.ActionPerformer = (action) =>
        {
            if (actionDetector.Source != null && actionDetector.Target && inventoryDetector.Source)
            {
                action.Execute(actionDetector.Source);
            }
        };

        return result;
    }
}

