public class LineAttackValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not LineAttackData ld)
            return ValidationResult.Fail("Invalid line data");

        if (!ld.IsValid)
            return ValidationResult.Fail("No enemy hit");

        return ValidationResult.Success();
    }
}