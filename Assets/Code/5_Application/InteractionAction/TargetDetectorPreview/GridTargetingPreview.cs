public class GridTargetingPreview : ITargetDetectorPreview
{
    private readonly IPlacementPreview _preview;
    private readonly TileTargetLogic _tileLogic;
    private readonly float _maxDistance;

    public GridTargetingPreview(IPlacementPreview preview, TileTargetLogic tileLogic, float maxDistance = 2f)
    {
        _preview = preview;
        _tileLogic = tileLogic;
        _maxDistance = maxDistance;
    }

    public void Setup(InteractionHandleContext context)
    {
        _preview.Hide();
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        var playerPos = context.PlayerPosition.Value;
        var pointerPos = context.PointerPosition.Value;
        var playerForward = context.PlayerDirection.Value;

        var previewInfos = _tileLogic.GetTargetTilePreview(playerPos, pointerPos, playerForward, _maxDistance);
        _preview.UpdatePreview(previewInfos);
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        var playerPos = context.PlayerPosition.Value;
        var pointerPos = context.PointerPosition.Value;
        var playerForward = context.PlayerDirection.Value;

        var previewInfos = _tileLogic.GetTargetTilePreview(playerPos, pointerPos, playerForward, _maxDistance);
        _preview.UpdatePreview(previewInfos);
    }

    public void DisablePreview()
    {
        _preview.Hide();
    }
}