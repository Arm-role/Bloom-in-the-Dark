using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractionTargetResolver
{
    private readonly ContactFilter2D _filter;
    private readonly Collider2D[] _colliderResults = new Collider2D[16];

    private readonly Tilemap _tilemap;
    private readonly TileLibrary _tileLibrary;
    private readonly GridConverter _gridConverter;

    public InteractionTargetResolver(LayerMask layerMask, Tilemap tilemap, TileLibrary tileLibrary, GridConverter gridConverter)
    {
        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = layerMask
        };

        _tilemap = tilemap;
        _tileLibrary = tileLibrary;
        _gridConverter = gridConverter;
    }

    public bool TryResolveTarget(Vector2 worldPosition, out InteractionTarget target)
    {
        target = default;

        int count = Physics2D.OverlapPoint(worldPosition, _filter, _colliderResults);
        if (count > 0)
        {
            Collider2D topCollider = _colliderResults[0];
            Debug.Log(topCollider.gameObject.name);
            target = new InteractionTarget(topCollider, worldPosition);
            return true;
        }

        if (_tilemap != null && _tileLibrary != null)
        {
            Vector3Int cellPos = _gridConverter.WorldToCell(worldPosition);
            TileBase tileBase = _tilemap.GetTile(cellPos);
            if(tileBase == null) return false;

            var tileData = _tileLibrary.GetTileData(tileBase);
            if (tileData != null)
            {
                Debug.Log(tileData.ToString());
                target = new InteractionTarget(tileData, worldPosition);
                return true;
            }
        }

        return false;
    }
}