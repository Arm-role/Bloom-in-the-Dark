public class ItemStrategyBundle
{
    public ITargetDetector Detector { get; }
    public ITargetDetectorPreview TargetDetectorPreview { get; }
    public ISkillIndicatorPreview SkillIndicatorPreview { get; }
    public ITargetValidator Validator { get; }
    public IActionPerformer Action { get; }
    public IDataProvider DataProvider { get; }


    public ItemStrategyBundle(
        ITargetDetector detector = null,
        ITargetDetectorPreview targetDetectorPreview = null,
        ISkillIndicatorPreview skillIndicatorPreview = null,
        ITargetValidator validator = null,
        IActionPerformer action = null,
        IDataProvider data = null)
    {
        Detector = detector;
        TargetDetectorPreview = targetDetectorPreview;
        SkillIndicatorPreview = skillIndicatorPreview;
        Validator = validator;
        Action = action;
        DataProvider = data;
    }
}