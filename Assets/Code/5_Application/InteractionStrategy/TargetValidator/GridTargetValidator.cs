public class GridTargetValidator : ITargetValidator
{
    public ValidationResult Validate(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        if (!target.IsValid)
            return ValidationResult.Fail("No target cells");

        if (ctx.ItemInstance?.Data is not ToolItem tool)
            return ValidationResult.Fail("Item is not a tool");

        foreach (var cell in target.Cells)
        {
            if (!ValidateCell(tool, cell, out var reason))
                return ValidationResult.Fail(reason);
        }

        return ValidationResult.Success();
    }

    private bool ValidateCell(
        ToolItem tool,
        WorldCell cell,
        out string reason)
    {
        reason = null;

        var upperTile = cell.GetUpperTile();

        switch (tool.Name)
        {
            case "Hoe":
                if (upperTile == null || upperTile.DisplayName != "Grass")
                {
                    reason = "Hoe can only be used on Grass";
                    return false;
                }
                break;

            case "Pickaxe":
                if (upperTile == null || upperTile.DisplayName != "Soil")
                {
                    reason = "Pickaxe can only be used on Soil";
                    return false;
                }
                break;
        }

        return true;
    }
}
