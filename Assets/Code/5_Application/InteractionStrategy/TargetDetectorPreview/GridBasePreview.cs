public class GridBasePreview : ITargetDetectorPreview
{
    private readonly IPlacementPreview _preview;
    private readonly PlacementController _placementController;

    public GridBasePreview(IPlacementPreview _placementPreview, PlacementController placementController)
    {
        _preview = _placementPreview;
        _placementController = placementController;
    }

    public void Setup(InteractionHandleContext context)
    {
        _preview.Hide();
    }
    public void EnablePreview(InteractionHandleContext context)
    {
        var tileInfo = _placementController.GetTargetTilePreview(context.PlayerPosition.Value, context.PointerPosition.Value);
        _preview.UpdatePreview(tileInfo);
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        var tileInfo = _placementController.GetTargetTilePreview(context.PlayerPosition.Value, context.PointerPosition.Value);
        _preview.UpdatePreview(tileInfo);
    }

    public void DisablePreview()
    {
        _preview.Hide();
    }
}