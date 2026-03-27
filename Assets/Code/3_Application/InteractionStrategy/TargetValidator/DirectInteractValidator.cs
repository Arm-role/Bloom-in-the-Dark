public class DirectInteractValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!target.IsValid)
            return ValidationResult.Fail("Target is invalid");

        if (target.Cells == null || target.Cells.Count != 1)
            return ValidationResult.Fail("DirectInteract requires exactly one cell");

        var cell = target.Cells[0];
        if (cell == null)
            return ValidationResult.Fail("Cell is null");

        if (!cell.HasAnyInteractable)
            return ValidationResult.Fail("No interactable on target cell");

        return ValidationResult.Success();
    }
}