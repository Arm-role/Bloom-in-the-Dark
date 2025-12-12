
using UnityEngine;

public class ActiveTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();

        bool canRemoveItem = false;
        bool canRemoveEnergy = false;

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

        result.PlayerEnergy = (playerEnergy) =>
        {
            if (canInteraction.IsValid)
            {
                canRemoveEnergy = playerEnergy.CanRemove(10f);
                Debug.Log("canRemoveEnergy " + canRemoveEnergy);

                if (canRemoveEnergy)
                {
                    playerEnergy.Remove(10f);
                }
            }
        };

        result.InventoryInteraction = (inventory) =>
        {
            canRemoveItem = inventory.CanRemoveItem(context.ItemInstance.Data, 1);
            if (canInteraction.IsValid && canRemoveItem && canRemoveEnergy)
            {
                int remaining = inventory.TryRemoveItem(context.ItemInstance.Data, 1);
            }
        };

        result.PlayerState = (playerData) =>
        {
            if (canInteraction.IsValid && canRemoveItem && canRemoveEnergy)
            {
                var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            }
        };

        result.ActionPerformer = async (action) =>
        {
            if (canInteraction.IsValid && canRemoveItem && canRemoveEnergy)
            {
                activeAction = await action.Execute(context, dataProvider);
            }
        };

        return result;
    }
}

