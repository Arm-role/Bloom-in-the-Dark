using UnityEngine;

public class ToolInteractionHandler : ITargetDetector
{
    private readonly PlacementController _placementController;

    public ToolInteractionHandler(PlacementController placementController)
    {
        _placementController = placementController;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.ItemData is IToolItemData)
            _placementController.Setup(Vector2Int.one, 2);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
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
