
public class AreaCircleValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not AreaCircleData areaData)
            return ValidationResult.Fail("Invalid data type");

        if(areaData.State != PlacementState.Valid)
            return ValidationResult.Fail("Don't Placement");
        return ValidationResult.Success();
    }
}