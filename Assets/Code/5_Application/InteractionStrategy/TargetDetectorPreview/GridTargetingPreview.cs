public class GridTargetingPreview : ITargetDetectorPreview
{
    private readonly IPlacementPreview _preview;
    private readonly TileTargetLogic _tileLogic;

    public GridTargetingPreview(IPlacementPreview preview, TileTargetLogic tileLogic)
    {
        _preview = preview;
        _tileLogic = tileLogic;
    }

    public void Setup(InteractionHandleContext context)
    {
        _preview.Hide();
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        var pointerPos = context.PointerPosition.Value;

        var previewInfos = _tileLogic.GetTargetTilePreview(pointerPos);

        _tileLogic.UpdateState(context.PlayerPosition.Value, context.PlayerDirection.Value);

        _preview.UpdatePreview(previewInfos);
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        _tileLogic.UpdateState(context.PlayerPosition.Value, context.PlayerDirection.Value);

        var pointerPos = context.PointerPosition.Value;

        var previewInfos = _tileLogic.GetTargetTilePreview(pointerPos);

        _preview.UpdatePreview(previewInfos);
    }

    public void DisablePreview()
    {
        _preview.Hide();
    }
}