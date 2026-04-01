public class SelfUseValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!ctx.PlayerPosition.HasValue)
            return ValidationResult.Fail("No player position");

        return ValidationResult.Success();
    }
}