using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldCell : IWorldCell
{
    public Vector3Int CellPos { get; }
    public Vector3 WorldCenter { get; }

    private Dictionary<ETileLayerType, IBaseTileData> _tiles = new();

    public CellActionRegistry ActionRegistry { get; }
        = new();

    private readonly ICellActionResolver _resolver;

    public bool IsWatered { get; set; }

    private GameObject _placedObject;
    public GameObject PlacedObject => _placedObject;
    public TileObstacle ObstacleObject { get; set; }

    public WorldCell(Vector3Int pos, Vector3 center, ICellActionResolver resolver)
    {
        CellPos = pos;
        WorldCenter = center;
        _resolver = resolver;
    }

    public bool HasObstacle
        => ObstacleObject != null &&
           ObstacleObject.BlocksMovement;

    public bool HasPlacedObject
        => _placedObject != null &&
           _placedObject.TryGetComponent<IPoolable<GameObject>>(out var poolable) &&
           poolable.IsAlive;

    public bool IsEmpty
        => _placedObject == null && _tiles.Count == 0;

    public bool HasAnyInteractable
        => ActionRegistry.HasAnyInteractable;

    public bool AddTile(
        ETileLayerType layer,
        IBaseTileData tileData)
    {
        if (_tiles.ContainsKey(layer))
            return false;

        _tiles[layer] = tileData;

        RebuildDerivedState();
        return true;
    }

    public bool RemoveTile(ETileLayerType layer)
    {
        if (!_tiles.Remove(layer))
            return false;

        RebuildDerivedState();
        return true;
    }

    public bool PlaceObject(GameObject obj)
    {
        if (_placedObject != null)
            return false;

        _placedObject = obj;

        RebuildDerivedState();
        return true;
    }

    public void RemoveObject()
    {
        if (_placedObject == null)
            return;

        if (_placedObject.TryGetComponent
                <IDestructible>(out var destructible))
            destructible.RequestDestruction();
        
        _placedObject = null;

        RebuildDerivedState();
    }

    private void RebuildDerivedState()
    {
        ActionRegistry.Clear();
        _resolver.Resolve(this, ActionRegistry);
    }

    public IBaseTileData GetUpperTile()
    {
        IBaseTileData upper = null;
        int highest = -1;

        foreach (var kv in _tiles)
        {
            int priority = TileLayerPriority.GetPriority(kv.Key);

            if (priority > highest)
            {
                highest = priority;
                upper = kv.Value;
            }
        }

        return upper;
    }

    public IReadOnlyList<IBaseTileData> Tiles
        => _tiles.Values.ToList();
}