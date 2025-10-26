public class ItemStrategyBundle
{
    public ITargetDetector Detector { get; }
    public ITargetValidator Validator { get; }
    public IActionPerformer Action { get; }
    public IDataProvider DataProvider { get; }

    public ItemStrategyBundle(
        ITargetDetector detector = null,
        ITargetValidator validator = null,
        IActionPerformer action = null,
        IDataProvider data = null)
    {
        Detector = detector;
        Validator = validator;
        Action = action;
        DataProvider = data;
    }
}