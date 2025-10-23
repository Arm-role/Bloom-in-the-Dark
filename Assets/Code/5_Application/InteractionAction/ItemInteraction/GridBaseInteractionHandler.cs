using UnityEngine;

public class GridBaseInteractionHandler : IInteractionHandle
{
    private readonly PlacementController _placementController;

    public GridBaseInteractionHandler(PlacementController placementController)
    {
        _placementController = placementController;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.ItemData is IBuildItemData build)
        _placementController.Setup(build.GridSize, 6);
    }

    public bool IntercationExcute(InteractionHandleContext context)
    {
        return _placementController.HandlePlacementClick(context.ItemInstance, context.PlayerPosition.Value, context.PointerPosition.Value);
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        _placementController.EnablePreview(context.PlayerPosition.Value, context.PointerPosition.Value);
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        _placementController.UpdatePreview(context.PlayerPosition.Value, context.PointerPosition.Value);
    }

    public void DisablePreview()
    {
        _placementController.DisablePreview();
    }
}