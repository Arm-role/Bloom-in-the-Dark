public class ItemStrategyBundle
{
    public IPointerResolver PointerResolver { get; }
    public ITargetDetector Detector { get; }
    public ITargetDetectorPreview TargetDetectorPreview { get; }
    public ISkillIndicatorPreview SkillIndicatorPreview { get; }
    public ITargetValidator Validator { get; }
    public IActionPerformer Action { get; }
    public IDataProvider DataProvider { get; }

    public ItemStrategyBundle(
        IPointerResolver pointerResolver = null,
        ITargetDetector detector = null,
        ITargetDetectorPreview targetDetectorPreview = null,
        ISkillIndicatorPreview skillIndicatorPreview = null,
        ITargetValidator validator = null,
        IActionPerformer action = null,
        IDataProvider data = null)
    {
        PointerResolver = pointerResolver;
        Detector = detector;
        TargetDetectorPreview = targetDetectorPreview;
        SkillIndicatorPreview = skillIndicatorPreview;
        Validator = validator;
        Action = action;
        DataProvider = data;
    }
}