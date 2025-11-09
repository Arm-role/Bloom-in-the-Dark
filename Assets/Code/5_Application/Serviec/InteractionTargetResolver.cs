using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractionTargetResolver
{
    private readonly ContactFilter2D _filter;
    private readonly Collider2D[] _colliderResults = new Collider2D[16];

    private readonly TileLibrary _tileLibrary;
    private readonly GridConverter _gridConverter;
    private readonly WorldTileManager _worldTileManager;

    public InteractionTargetResolver(
        LayerMask layerMask,
        TileLibrary tileLibrary,
        GridConverter gridConverter,
        WorldTileManager worldTileManager)
    {
        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = layerMask
        };

        _tileLibrary = tileLibrary;
        _gridConverter = gridConverter;
        _worldTileManager = worldTileManager;
    }

    public bool TryResolveTarget(Vector2 worldPosition, out InteractionTargetContext target)
    {
        target = default;

        int count = Physics2D.OverlapPoint(worldPosition, _filter, _colliderResults);
        if (count > 0)
        {
            Collider2D topCollider = _colliderResults[0];
            Debug.Log(topCollider.gameObject.name);
            target = new InteractionTargetContext(topCollider, worldPosition);
            return true;
        }

        Vector3Int cellPos = _gridConverter.WorldToCell(worldPosition);
        var tileState = _worldTileManager.GetTileState(cellPos);

        if (tileState != null)
        {
            if(tileState.placedObject != null) Debug.Log(tileState.placedObject.gameObject.name);
            target = new InteractionTargetContext(tileState, worldPosition);
            return true;
        }

        return false;
    }
}