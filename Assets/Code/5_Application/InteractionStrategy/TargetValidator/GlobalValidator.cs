using Codice.Client.BaseCommands.Merge.Xml;
using UnityEngine;
using static UnityEngine.Animations.AimConstraint;

public class GlobalValidator : ITargetValidator
{
    private readonly InteractionRuleSet _ruleSet;

    public GlobalValidator(InteractionRuleSet ruleSet)
    {
        _ruleSet = ruleSet;
    }

    public ValidationResult Validate(IDataProvider data)
    {
        if (data is not GlobalData globalData)
            return ValidationResult.Fail("Invalid data type GridTargetingData");

        if (!globalData.PointerPosition.HasValue)
            return ValidationResult.Fail("No pointer position");

        var target = globalData.Target;

        if (!target.IsValid)
            return ValidationResult.Fail("No tile found");

        var itemData = globalData.ItemInstance.ItemData;

        if (itemData is ToolItem)
            return ValidationResult.Fail("Item Is Tool");

        if (target.IsObject)
        {
            var interacableObject = target.WorldInteractable;
            Debug.Log(itemData.Type + "&&" + interacableObject.Type);

            if (!_ruleSet.CanInteract(itemData.Type, interacableObject.Type))
                return ValidationResult.Fail("Can't Interact");
        }
        else if (target.IsTile)
        {
            var interacableTile = target.TileState;
            if (!_ruleSet.CanInteract(itemData.Type, interacableTile.InteractableType))
                return ValidationResult.Fail("Can't Interact");
        }

        return ValidationResult.Success();
    }
}