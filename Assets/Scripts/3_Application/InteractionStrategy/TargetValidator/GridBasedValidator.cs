public class GridBasedValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!target.IsValid)
            return ValidationResult.Fail("No target cells");

        foreach (var cell in target.Cells)
        {
            if (cell == null)
                return ValidationResult.Fail("Invalid cell");

            if (cell.HasPlacedObject)
                return ValidationResult.Fail("Cell is occupied");

            if (!cell.ZoneFlags.HasFlag(CellZoneFlags.PlacementAllowed))
                return ValidationResult.Fail("Not in a placement zone");
        }

        return ValidationResult.Success();
    }
}
