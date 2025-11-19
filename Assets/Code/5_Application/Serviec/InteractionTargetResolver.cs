using System.Collections.Generic;
using UnityEngine;
public class InteractionTargetResolver
{
    private readonly Collider2D[] _colliderResults = new Collider2D[16];

    private readonly GridConverter _gridConverter;
    private readonly WorldTileManager _worldTileManager;

    public InteractionTargetResolver(
        GridConverter gridConverter,
        WorldTileManager worldTileManager)
    {
        _gridConverter = gridConverter;
        _worldTileManager = worldTileManager;
    }

    public bool TryResolveTarget(
        Vector2 worldPosition,
        ETargetResolveType typeFlags,
        out InteractionTargetContext target)
    {
        LayerMask mask = BuildLayerMask(typeFlags);
        return TryResolveTarget(worldPosition, mask, out target);
    }

    public bool TryResolveTarget(
        Vector2 worldPosition,
        LayerMask customMask,
        out InteractionTargetContext target)
    {
        target = default;

        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = customMask,
            useTriggers = true
        };

        int count = Physics2D.OverlapPoint(worldPosition, filter, _colliderResults);

        if (count > 0)
        {
            Collider2D topCollider = _colliderResults[0];
            var interactable = topCollider.GetComponent<IWorldInteractable>();
            target = new InteractionTargetContext(interactable, worldPosition);
            return true;
        }

        Vector3Int cellPos = _gridConverter.WorldToCell(worldPosition);
        var tileState = _worldTileManager.GetTileState(cellPos);

        if (tileState != null)
        {
            target = new InteractionTargetContext(tileState, worldPosition);
            return true;
        }

        return false;
    }

    public bool TryResolveAllTargets(
       Vector2 worldPosition,
       ETargetResolveType typeFlags,
       out List<InteractionTargetContext> targets)
    {
        LayerMask mask = BuildLayerMask(typeFlags);
        return TryResolveAllTargets(worldPosition, mask, out targets);
    }

    public bool TryResolveAllTargets(
    Vector2 worldPos,
    LayerMask mask,
    out List<InteractionTargetContext> targets)
    {
        targets = new List<InteractionTargetContext>();

        var filter = new ContactFilter2D { useLayerMask = true, layerMask = mask , useTriggers = true};
        int count = Physics2D.OverlapPoint(worldPos, filter, _colliderResults);

        for (int i = 0; i < count; i++)
        {
            var col = _colliderResults[i];
            var interactable = col.GetComponent<IWorldInteractable>();

            if (interactable != null)
            {
                targets.Add(new InteractionTargetContext(interactable, worldPos));
            }
        }

        var cellPos = _gridConverter.WorldToCell(worldPos);
        var tileState = _worldTileManager.GetTileState(cellPos);

        if (tileState != null && tileState.WorldInteractable != null)
        {
            targets.Add(new InteractionTargetContext(tileState, worldPos));
        }

        return targets.Count > 0;
    }

    private LayerMask BuildLayerMask(ETargetResolveType flags)
    {
        int mask = 0;

        if (flags.HasFlag(ETargetResolveType.Enemy))
            mask |= LayerMask.GetMask("Enemy");

        if (flags.HasFlag(ETargetResolveType.Ally))
            mask |= LayerMask.GetMask("Ally");

        if (flags.HasFlag(ETargetResolveType.Interactable))
            mask |= LayerMask.GetMask("Interactable");

        if (flags.HasFlag(ETargetResolveType.Item))
            mask |= LayerMask.GetMask("Item");

        if (flags.HasFlag(ETargetResolveType.Ground))
            mask |= LayerMask.GetMask("Ground");

        return mask;
    }
}