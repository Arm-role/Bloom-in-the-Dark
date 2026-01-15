using System.Threading.Tasks;
using UnityEngine;

public class PlantSeedAction : ICellAction
{
    public InteractionStage Stage => InteractionStage.Pre;

    public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
    {
        if (cell.HasPlacedObject)
            return Task.FromResult(false);
        
        if (!intent.Is(
                EInteractionIntentType.Plant))
            return Task.FromResult(false);
        return Task.FromResult(true);
    }

    public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
    {
        Debug.Log("Planting seed");

        var result = new WorldAction();

        if (intent.SourceItem.HasProperty(EItemProperty.PrefabName))
        {
            result.PlaceObject = intent.SourceItem.GetProperty<string>(EItemProperty.PrefabName);
            return InteractionResult.Consumed(result);
        }

        return InteractionResult.Blocked(result);
    }
}