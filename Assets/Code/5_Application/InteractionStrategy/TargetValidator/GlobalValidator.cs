using Codice.CM.Common;
using UnityEngine;

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

        var type = (globalData.ItemInstance != null) ? globalData.ItemInstance.Data.Type : EItemType.None;

        if (target.IsObject)
        {
            var interacableObject = target.WorldInteractable;

            Debug.Log("IsObject " + type + " " + interacableObject.Type);
            if (!_ruleSet.CanInteract(type, interacableObject.Type))
                return ValidationResult.Fail("Can't Interact");
        }
        else if (target.IsTile)
        {
            var interacableTile = target.TileState;

            Debug.Log("IsTile " + type + " " + interacableTile.WorldInteractableType);
            if (!_ruleSet.CanInteract(type, interacableTile.WorldInteractableType))
                return ValidationResult.Fail("Can't Interact");
        }

        return ValidationResult.Success();
    }
}