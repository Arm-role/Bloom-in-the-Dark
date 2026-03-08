public interface ITargetValidator
{
    ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target);
}