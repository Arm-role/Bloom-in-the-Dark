
using UnityEngine;

public class ActiveTargetAction : IItemBehavior
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
                if (canInteraction.Reason != null) Debug.Log(canInteraction.Reason);
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

        result.PlayerData = (playerData) =>
        {
            if (canInteraction.IsValid && canRemoveItem)
            {
                var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            }
        };

        result.ActionPerformer = async (action) =>
        {
            if (canInteraction.IsValid && canRemoveItem)
            {
                activeAction = await action.Execute(context, dataProvider);
            }
        };

        return result;
    }
}

