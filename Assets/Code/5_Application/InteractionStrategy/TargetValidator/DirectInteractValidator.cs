
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

        var itemData = directInteractData.ItemInstance.Data;

        if (itemData is SeedItem)
        {
            if (!directInteractData.Target.IsTile)
                return ValidationResult.Fail("Tile Don't Set");

            var groundTile = target.TileState.GetTile(ETileLayerType.Interactable);
            if (groundTile == null || groundTile.DisplayName != "Soil")
                return ValidationResult.Fail("Pickaxe can only be used on Soil");
        }

        if (target.TileState.PlacedObject != null)
        {
            Debug.Log((target.TileState.PlacedObject != null) + "&&" + (target.TileState.PlacedObject.IsAlive));
        }

        if (target.TileState.IsOccupied)
            return ValidationResult.Fail("Tile occupied");

        return ValidationResult.Success();
    }
}
