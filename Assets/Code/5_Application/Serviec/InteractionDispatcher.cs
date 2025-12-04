using Codice.CM.Client.Differences;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public class InteractionDispatcher
{
    private readonly InteractionRuleSet _ruleSet;
    private readonly DefaultLayerPriorityRuleSO _defaultPriorityRule;

    private readonly InteractionTargetResolver _target;
    private readonly InteractionPriorityResolver _priorityResolver = new();

    public InteractionDispatcher(
        InteractionRuleSet ruleSet,
        DefaultLayerPriorityRuleSO defaultLayerPriorityRuleSO,
        InteractionTargetResolver target)
    {
        _target = target;
        _ruleSet = ruleSet;
        _defaultPriorityRule = defaultLayerPriorityRuleSO;
    }

    public bool TryInteract(
        InteractionHandleContext context,
        Vector2 targetPosition,
        ETargetResolveType typeFlags,
        out InteractionTargetContext target)
    {
        target = default;

        if (!_target.TryResolveAllTargets(targetPosition, typeFlags, out var allTargets))
            return false;

        InteractionTargetContext? best = null;

        if (context.InputActionType == InputActionType.Primary)
        {
            best = _priorityResolver.ResolveBest(allTargets, context.ItemInstance.Data.PriorityRule);
        }
        else if (context.InputActionType == InputActionType.Secondary)
        {
            best = _priorityResolver.ResolveBest(allTargets, _defaultPriorityRule);
        }

        if (best == null) return false;

        target = best.Value;

        //if (target.IsValid && context.InputActionType == InputActionType.Secondary)
        //{
        //    var worldType = GetWorldTypeFromTarget(target);
        //    if (_ruleSet.CanInteract(context.ItemInstance.Data.Type, worldType))
        //    {
        //        return EInteractionMode.UseGlobal;
        //    }
        //}

        return true;
    }

    private ETileBlockType GetWorldTypeFromTarget(InteractionTargetContext target)
    {
        if (target.WorldInteractable != null)
        {
            var comp = target.WorldInteractable;
            if (comp != null) return comp.Type;
        }
        if (target.TileState != null)
        {
            return target.TileState.WorldInteractableType;
        }

        return ETileBlockType.None;
    }
}