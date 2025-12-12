public class GridTargetingValidator : ITargetValidator
{
    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not GridTargetingData gridTargetingData)
            return ValidationResult.Fail("Invalid data type GridTargetingData");

        if (!gridTargetingData.PointerPosition.HasValue)
            return ValidationResult.Fail("No pointer position");

        var tileState = gridTargetingData.TileTarget;
        if (tileState == null)
            return ValidationResult.Fail("No tile found");

        var itemData = gridTargetingData.ItemInstance.Data;

        if (itemData is not ToolItem toolItem)
            return ValidationResult.Fail("Item Not Tool");

        if (toolItem.Name == "Hoe")
        {
            var tilebaseData = tileState.GetUpperTile();
            if (tilebaseData == null || tilebaseData.DisplayName != "Grass")
                return ValidationResult.Fail("Hoe can only be used on Grass");
        }

        else if (toolItem.Name == "Pickaxe")
        {
            var tilebaseData = tileState.GetUpperTile();
            if (tilebaseData == null || tilebaseData.DisplayName != "Soil")
                return ValidationResult.Fail("Hoe can only be used on Soil");
        }

        return ValidationResult.Success();
    }
}
