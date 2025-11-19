using UnityEngine;

public class GridBaseDetactor : ITargetDetector
{
    private readonly PlacementController _placementController;
    private readonly float _maxDistance;

    public GridBaseDetactor(PlacementController placementController, float maxDistance = 6)
    {
        _placementController = placementController;
        _maxDistance = maxDistance;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.Data is IBuildItemData build)
            _placementController.Setup(build.GridSize, _maxDistance);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var tileplacement = _placementController.HandlePlacementClick(context.ItemInstance, context.PlayerPosition.Value, context.PointerPosition.Value);
        return new GridBaseData(placementInfos: tileplacement);
    }
}
