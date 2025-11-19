
using UnityEngine;

public class InteractionTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();

        ValidationResult canInteraction = ValidationResult.Fail("Dont' Set");
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

        result.PlayerData = (playerData) =>
        {
            if (canInteraction.IsValid)
            {
                var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
                playerData.Look(dir);
            }
        };

        result.ActionPerformer = async (action) =>
        {
            if (canInteraction.IsValid)
            {
                activeAction = await action.Execute(context,dataProvider);
            }
            else
            {
                Debug.Log(canInteraction.Reason);
            }
        };

        Debug.Log("Global");

        return result;
    }
}
