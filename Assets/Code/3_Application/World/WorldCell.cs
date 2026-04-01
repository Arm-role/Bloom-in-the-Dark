using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldCell : IWorldCell
{
  public Vector3Int CellPos { get; }
  public Vector3 WorldCenter { get; }

  private Dictionary<ETileLayerType, IBaseTileData> _tiles = new();

  public ActionRegistry ActionRegistry { get; }
    = new();

  private readonly ICellActionResolver _resolver;

  public bool IsWatered { get; set; }

  private CellObject? _object;
  public GameObject Object => _object?.Object;

  public WorldCell(Vector3Int pos, Vector3 center, ICellActionResolver resolver)
  {
    CellPos = pos;
    WorldCenter = center;
    _resolver = resolver;
  }

  public bool HasPlacedObject =>
    _object.HasValue &&
    _object.Value.Object.TryGetComponent<IPoolable<GameObject>>(out var poolable) &&
    poolable.IsAlive;

  public bool BlocksMovement
  {
    get
    {
      if (!_object.HasValue)
        return false;

      var cellObj = _object.Value;

      if (!cellObj.Object.TryGetComponent<WorldObject>(out var ob))
        return false;

      if (cellObj.HasFlag(CellObjectFlags.Obstacle))
        return ob.ObstacleBlocksMovement;

      if (cellObj.HasFlag(CellObjectFlags.Placement))
        return ob.PlacementBlocksMovement;

      return false;
    }
  }

  public bool BlocksVision
  {
    get
    {
      if (!_object.HasValue)
        return false;

      var cellObj = _object.Value;

      if (!cellObj.Object.TryGetComponent<WorldObject>(out var ob))
        return false;

      if (cellObj.HasFlag(CellObjectFlags.Obstacle))
        return ob.ObstacleBlocksVision;

      if (cellObj.HasFlag(CellObjectFlags.Placement))
        return ob.PlacementBlocksVision;

      return false;
    }
  }

  public bool IsEmpty
    => !_object.HasValue && _tiles.Count == 0;

  public bool CanInteract
    => ActionRegistry.HasAnyInteractable &&
       ZoneFlags.HasFlag(CellZoneFlags.SafeZone);

  public CellZoneFlags ZoneFlags { get; private set; }

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

  public void AddObjectFlag(CellObjectFlags flag)
  {
    if (!_object.HasValue)
      return;

    var obj = _object.Value;
    obj.Flags |= flag;

    _object = obj;
  }


  public bool PlaceObject(GameObject obj, CellObjectFlags flags)
  {
    if (_object.HasValue)
      return false;

    _object = new CellObject(obj, flags);

    RebuildDerivedState();
    return true;
  }

  public void RemoveObject()
  {
    if (!_object.HasValue)
      return;

    var obj = _object.Value.Object;

    if (obj.TryGetComponent<IDestructible>(out var destructible))
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
    return _tiles.ContainsKey(layer) && _tiles.Count == 1;
  }

  public bool HasAnyLayerExcept(ETileLayerType layer)
  {
    return _tiles.Any(kv => kv.Key != layer);
  }

  public IReadOnlyList<IBaseTileData> Tiles
    => _tiles.Values.ToList();

  public void SetZoneFlags(CellZoneFlags flags)
  {
    ZoneFlags = flags;
    Debug.Log(ZoneFlags);
  }
}
