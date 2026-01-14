using UnityEngine;

public class AreaCircleValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!target.IsValid)
            return ValidationResult.Fail("Invalid target");

        if (ctx.ItemInstance is not PlantItemInstance plant)
            return ValidationResult.Fail("Item is not PlantItemInstance");

        if (plant.Data is not PlantItem)
            return ValidationResult.Fail("Item data is not PlantItem");

        if (target.Extra is not Vector2)
            return ValidationResult.Fail("No area center");

        return ValidationResult.Success();
    }
}