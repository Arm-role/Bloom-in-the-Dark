using UnityEngine;

public class GridTargetAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();
        var process = new ProcessState<IDataProvider, bool>();

        result.TargetDetector = (handle) =>
        {
            process.Source = handle.IntercationExcute(context);
        };

        result.PlayerData = (playerData) =>
        {
            if (process.Source != null)
            {
                if (process.Source is GridTargetingData data && data.PlacementInfos != null)
                {
                    if (data.PlacementInfos.Count > 0)
                    {
                        Vector2 pointerPosition = data.PlacementInfos[0].WorldPosition;
                        var dir = (pointerPosition - context.PlayerPosition.Value).normalized;
                        playerData.Look(dir);
                    }
                }
            }
        };

        result.ActionPerformer = (action) =>
        {
            if (process.Source != null)
            {
                action.Execute(process.Source);
            }
        }; 

        return result;
    }
}
