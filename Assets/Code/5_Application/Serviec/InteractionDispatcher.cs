using UnityEngine;

public class InteractionDispatcher
{
    private readonly InteractionTargetResolver _target;
    private readonly InteractionRuleSet _ruleSet;

    public InteractionDispatcher(InteractionTargetResolver target, InteractionRuleSet ruleSet)
    {
        _target = target;
        _ruleSet = ruleSet;
    }

    public EInteractionMode TryInteract(
        InteractionHandleContext context,
        Vector2 targetPosition,
        ETargetResolveType typeFlags,
        out InteractionTargetContext target)
    {
        if (!_target.TryResolveTarget(targetPosition, typeFlags, out target))
            return EInteractionMode.None;

        if (target.IsValid)
        {
            var worldType = GetWorldTypeFromTarget(target);
            if (_ruleSet.CanInteract(context.ItemInstance.ItemData.Type, worldType))
            {
                return EInteractionMode.UseGlobal;
            }
        }

        return EInteractionMode.UseItem;
    }

    private EWorldInteractableType GetWorldTypeFromTarget(InteractionTargetContext target)
    {
        if (target.WorldInteractable != null)
        {
            var comp = target.WorldInteractable;
            if (comp != null) return comp.Type;
        }
        if (target.TileState != null)
        {
            return target.TileState.InteractableType;
        }

        return EWorldInteractableType.None;
    }
}