using System.Collections.Generic;
using UnityEngine;

public class PlacementAction : IActionPerformer
{
    private TilemapInteractionController _interactionController;

    public PlacementAction(TilemapInteractionController interactionController)
    {
        _interactionController = interactionController;
    }
    public void Setup()
    {
        _interactionController.SetUp();
    }
    public void Execute(IDataProvider data)
    {
        if (data is not GridInteractionData gridData) return;

        HashSet<Vector2> placedPositions = new HashSet<Vector2>();

        foreach (var placePoint in gridData.PlacementInfos)
        {
            if (placePoint.State == PlacementState.Valid && placedPositions.Add(placePoint.WorldPosition))
            {
                _interactionController.PlaceTile(placePoint.WorldPosition);
            }
        }
    }
}