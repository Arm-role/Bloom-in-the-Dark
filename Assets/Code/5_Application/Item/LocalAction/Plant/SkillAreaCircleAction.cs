
using UnityEngine;

public class SkillAreaCircleAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();

        bool canRemoveItem = false;
        ValidationResult canInteraction = ValidationResult.Fail("NotSet");
        bool activeAction = false;
        bool pressSkillModifer = false;
        IDataProvider dataProvider = null;

        result.ModifierInput = (auxiliaryInput) =>
        {
            pressSkillModifer = auxiliaryInput.ExecuteActions == InputActionType.SkillModifierHeld;
        };

        result.TargetDetector = (handle) =>
        {
            if (pressSkillModifer)
            {
                dataProvider = handle.IntercationExcute(context);
            }
        };

        result.TargetValidator = (validator) =>
        {
            if (dataProvider != null && dataProvider.IsValid)
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
    public ActionExecutionResult SecondaryActionExecute(InteractionHandleContext context)
    {
        throw new System.NotImplementedException();
    }
}

