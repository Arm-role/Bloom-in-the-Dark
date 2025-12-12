public class ProximityValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not ProximityInteractionData pd)
            return ValidationResult.Fail("Invalid proximity data");

        if (!pd.IsValid)
            return ValidationResult.Fail("No object nearby");

        return ValidationResult.Success();
    }
}