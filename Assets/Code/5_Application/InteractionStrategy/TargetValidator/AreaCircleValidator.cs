
public class AreaCircleValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not AreaCircleData areaData)
            return ValidationResult.Fail("Invalid data type");

        if(areaData.State != PlacementState.Valid)
            return ValidationResult.Fail("Don't Placement");

        if (data is not AreaCircleData areaCircleData) 
            return ValidationResult.Fail("Don't AreaCircleData");

        if (areaCircleData.ItemInstance is not PlantItemInstance plantItemInstance) 
            return ValidationResult.Fail("Don't PlantItemInstance");

        if (areaCircleData.ItemInstance.Data is not PlantItem plantItem) 
            return ValidationResult.Fail("Don't PlantItem");

        if (areaCircleData.PointerPosition.Value == null) 
            return ValidationResult.Fail("PointerPosition Null");

        return ValidationResult.Success();
    }
}