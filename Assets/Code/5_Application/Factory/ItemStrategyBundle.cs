public sealed class ItemStrategyBundle
{
    public TargetingStrategy Targeting { get; }
    public IInteractionPreview Preview { get; }
    public IActionPerformer Action { get; }

    public ItemStrategyBundle(
        TargetingStrategy targeting,
        IInteractionPreview preview,
        IActionPerformer action)
    {
        Targeting = targeting;
        Preview = preview;
        Action = action;
    }
}
public class TargetingStrategy
{
    public ITargetingConfigProvider ConfigProvider;
    public ITargetStrategy Strategy;
    public ITargetValidator Validator;
}