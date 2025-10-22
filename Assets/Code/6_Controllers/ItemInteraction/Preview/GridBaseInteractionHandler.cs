using UnityEngine;

public class GridBaseInteractionHandler : IInteractionHandle
{
    private readonly PlacementController _placementController;

    public GridBaseInteractionHandler(PlacementController placementController)
    {
        _placementController = placementController;
    }

    public void Setup(IItemInstance itemInstance)
    {
        if (itemInstance.ItemData is IBuildItemData build)
        _placementController.Setup(build.GridSize, 6);
    }

    public bool IntercationExcute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null)
    {
       return _placementController.HandlePlacementClick(itemInstance, playerPosition, pointerPosition);
    }

    public void EnablePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
        _placementController.EnablePreview(playerPosition, pointerPosition);
    }

    public void UpdatePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
        _placementController.UpdatePreview(playerPosition, pointerPosition);
    }

    public void DisablePreview()
    {
        _placementController.DisablePreview();
    }
}