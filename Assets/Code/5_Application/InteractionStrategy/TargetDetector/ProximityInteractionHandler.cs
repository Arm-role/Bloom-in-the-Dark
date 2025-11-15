public class ProximityInteractionHandler : ITargetDetector
{
    private readonly ProximityDetector _detector;

    public ProximityInteractionHandler(ProximityDetector detector)
    {
        _detector = detector;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.ItemData is ToolItem tool)
            _detector.Setup(tool.AttackRange);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        return _detector.DetectTargets(context.PlayerPosition.Value);
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        _detector.EnablePreview(context.PlayerPosition.Value);
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        _detector.UpdatePreview(context.PlayerPosition.Value);
    }

    public void DisablePreview()
    {
        _detector.DisablePreview();
    }
}