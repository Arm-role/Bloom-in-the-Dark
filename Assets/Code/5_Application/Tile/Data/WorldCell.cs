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

    private GameObject _object;
    public GameObject Object => _object;

    public WorldCell(Vector3Int pos, Vector3 center, ICellActionResolver resolver)
    {
        CellPos = pos;
        WorldCenter = center;
        _resolver = resolver;
    }

    public bool HasPlacedObject
        => _object != null &&
           _object.TryGetComponent<IPoolable<GameObject>>(out var poolable) &&
           poolable.IsAlive;
    
    public bool BlocksMovement =>
        _object != null &&
        _object.TryGetComponent<WorldObject>(out var ob) &&
        ob.BlocksMovement;

    public bool BlocksVision =>
        _object != null &&
        _object.TryGetComponent<WorldObject>(out var ob) &&
        ob.BlocksVision;

    public bool IsEmpty
        => _object == null && _tiles.Count == 0;

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
        if (_object != null)
            return false;

        _object = obj;

        RebuildDerivedState();
        return true;
    }

    public void RemoveObject()
    {
        if (_object == null)
            return;

        if (_object.TryGetComponent
                <IDestructible>(out var destructible))
            destructible.RequestDestruction();
        
        _object = null;

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

    public bool HasTile<T>() where T : IBaseTileData
    {
        foreach (var tile in _tiles.Values)
        {
            if (tile is T)
                return true;
        }
        return false;
    }
    
    public bool IsSingleLayer(ETileLayerType layer)
    {
        Debug.Log(_tiles.Count);
        return _tiles.ContainsKey(layer) && _tiles.Count == 1;
    }
    
    public bool HasAnyLayerExcept(ETileLayerType layer)
    {
        return _tiles.Any(kv => kv.Key != layer);
    }

    
    public IReadOnlyList<IBaseTileData> Tiles
        => _tiles.Values.ToList();
}