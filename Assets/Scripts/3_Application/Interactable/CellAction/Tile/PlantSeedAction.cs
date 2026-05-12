using System.Threading.Tasks;
using UnityEngine;

public class PlantSeedAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType { get; }

  public PlantSeedAction(ETargetType targetType)
  {
    TargetType = targetType;
  }

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    Debug.Log("HasPlacedObject " + cell.HasPlacedObject);
    if (cell.HasPlacedObject)
      return Task.FromResult(false);

    if (!intent.Is(
          EInteractionIntentType.Plant))
      return Task.FromResult(false);

    return Task.FromResult(true);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    var placementProfile = intent.SourceItem.Data.PlacementProfile;

    if (placementProfile != null && placementProfile.ObjectKey != null)
    {
      result.PlaceObjectId = placementProfile.ObjectKey.RuntimeTag.Hash;
      return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None, InteractionCost.OneItem);
    }

    return InteractionResult.Blocked(cell, result, TargetType);
  }
}