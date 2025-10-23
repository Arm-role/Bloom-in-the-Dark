using System.Collections;
using UnityEngine;

public class TilemapPlacementAction : IItemAction
{
    private TilemapInteractionController _interactionController;

    public TilemapPlacementAction(TilemapInteractionController interactionController)
    {
        _interactionController = interactionController;
    }

    public void Setup()
    {
        _interactionController.SetUp();
    }

    public void Action(Vector2 placePoint)
    {
        _interactionController.PlaceTile(placePoint);
    }
}