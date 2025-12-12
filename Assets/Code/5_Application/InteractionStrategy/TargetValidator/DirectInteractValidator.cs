
using UnityEngine;

public class DirectInteractValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not DirectInteractData directInteractData)
            return ValidationResult.Fail("Invalid data type");

        if (directInteractData.PointerPosition.Value == null)
            return ValidationResult.Fail("Position Don't Set");

        var target = directInteractData.Target;

        if (!target.IsValid)
            return ValidationResult.Fail("No Target");

        return ValidationResult.Success();
    }
}
