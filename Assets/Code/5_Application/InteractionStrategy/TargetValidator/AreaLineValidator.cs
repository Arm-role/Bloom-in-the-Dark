using UnityEngine;

public class AreaLineValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!target.IsValid)
            return ValidationResult.Fail("Invalid target");

        if (!ctx.ItemInstance.HasProperty(EItemProperty.SkillName))
            return ValidationResult.Fail("Item is not SkillName");
        
        if (target.Extra is not Vector2)
            return ValidationResult.Fail("No line end");

        return ValidationResult.Success();
    }
}