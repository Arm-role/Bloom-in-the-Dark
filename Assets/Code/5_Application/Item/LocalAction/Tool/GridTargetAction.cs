using UnityEngine;

public class GridTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();

        bool canRemoveEnergy = false;

        ValidationResult canInteraction = new();
        bool activeAction = false;
        IDataProvider dataProvider = null;

        result.TargetDetector = (detector) =>
        {
            dataProvider = detector.IntercationExcute(context);
        };

        result.TargetValidator = (validator) =>
        {
            if (dataProvider.IsValid)
            {
                canInteraction = validator.Validate(dataProvider);
            }
        };

        result.PlayerEnergy = (playerEnergy) =>
        {
            if (canInteraction.IsValid)
            {
                canRemoveEnergy = playerEnergy.CanRemove(10f);

                if (canRemoveEnergy)
                {
                    playerEnergy.Remove(10f);
                }
            }
        };

        result.PlayerState = (playerData) =>
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
