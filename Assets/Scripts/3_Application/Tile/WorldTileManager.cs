using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class WorldTileManager : MonoBehaviour
{
  // -----------------------------
  // Dependencies
  // -----------------------------
  private ITileLibrary _tileLibrary;
  private ICellActionResolver _actionResolver;
  private WorldZoneManager _zoneManager;

  private TileLayerRenderer _renderer;
  private GridSpatialQuery _spatialQuery;
  public IGridConverter GridConverter { get; private set; }


  // -----------------------------
  // World Data
  // -----------------------------
  private readonly WorldCellRegistry _registry = new();

  private Dictionary<GameObject, List<Vector3Int>> _objectCells
    = new();

  public IEnumerable<GameObject> Objects => _objectCells.Keys;

  // -----------------------------
  // Initialization
  // -----------------------------
  public void Initialize(
    List<TilemapLayer> tilemapLayers,
    ITileLibrary tileLibrary,
    IGridConverter gridConverter,
    ICellActionResolver actionResolver,
    WorldZoneManager zoneManager)
  {
    _tileLibrary = tileLibrary;
    GridConverter = gridConverter;
    _actionResolver = actionResolver;
    _zoneManager = zoneManager;
    _renderer = new TileLayerRenderer(tilemapLayers);
    _spatialQuery = new GridSpatialQuery(_registry.Cells, GridConverter);

    _registry.Clear();

    foreach (var layer in tilemapLayers)
    {
      if (layer.tilemap == null) continue;
      ScanTileLayer(layer.layerType, layer.tilemap);
    }

    _zoneManager.ZoneChanged += OnZoneChanged;

    RegisterMapObjects();
  }

  private void ScanTileLayer(ETileLayerType layerType, Tilemap tilemap)
  {
    var bounds = tilemap.cellBounds;

    for (int x = bounds.xMin; x < bounds.xMax; x++)
    {
      for (int y = bounds.yMin; y < bounds.yMax; y++)
      {
        var cellPos = new Vector3Int(x, y, 0);
        var tile = tilemap.GetTile(cellPos);
        if (tile == null)
          continue;

        var cell = GetOrCreateCell(cellPos);
        var tileData = _tileLibrary.GetTileData(tile);
        cell.AddTile(layerType, tileData);
      }
    }

    TileDomainEvents.TileScanCompleted();
  }

  // -----------------------------
  // MapObjects Scan
  // -----------------------------
  public void RegisterMapObjects()
  {
    foreach (var obj in _objectCells.Keys.ToList())
    {
      RemoveObject(obj);
    }

    _objectCells.Clear();

    var worldObjects = FindObjectsOfType<WorldObject>();

    foreach (var ob in worldObjects)
    {
      TryPlaceObject(ob.gameObject);
    }

    TileDomainEvents.ObstacleScanCompleted();
  }


  public WorldCell GetCell(Vector3Int cellPos)
    => _registry.Get(cellPos);

  public WorldCell GetCellFromWorld(Vector3 worldPos)
    => _registry.Get(GridConverter.WorldToCell(worldPos));

  public IEnumerable<WorldCell> GetAllCells()
    => _registry.All;

  private WorldCell GetOrCreateCell(Vector3Int cellPos)
  {
    var existing = _registry.Get(cellPos);
    if (existing != null)
      return existing;

    var cell = new WorldCell(
      cellPos,
      GridConverter.GetCellCenterWorld(cellPos),
      _actionResolver);

    UpdateCellZone(cell);

    _registry.Add(cellPos, cell);
    return cell;
  }

  private void UpdateCellZone(WorldCell cell)
  {
    if (_zoneManager == null)
      return;

    var flags = _zoneManager.GetFlags(cell.WorldCenter);
    cell.SetZoneFlags(flags);
  }

  private void OnDestroy()
  {
    if (_zoneManager != null)
      _zoneManager.ZoneChanged -= OnZoneChanged;
  }

  private void OnZoneChanged(IWorldZone zone) => ApplyZone(zone);

  [ContextMenu("Refresh All Zones")]
  public void RefreshAllZones()
  {
    if (_zoneManager == null)
      return;

    foreach (var cell in _registry.All)
    {
      UpdateCellZone(cell);
    }
  }

  public void ApplyZone(IWorldZone zone)
  {
    var cells = GetCellsInRadius(zone.Center, zone.Radius);

    foreach (var cell in cells)
    {
      UpdateCellZone(cell);
    }
  }

  // -----------------------------
  // Runtime Tile Modification
  // -----------------------------
  public bool TryAddTile(
    Vector3Int cellPos,
    ETileLayerType layer,
    IBaseTileData tileData)
  {
    var cell = GetOrCreateCell(cellPos);

    if (!cell.AddTile(layer, tileData))
      return false;

    _renderer.SetTile(
      cellPos,
      layer,
      tileData.Tiles.FirstOrDefault());

    return true;
  }

  public bool TryRemoveTile(
    Vector3Int cellPos,
    ETileLayerType layer)
  {
    var cell = _registry.Get(cellPos);
    if (cell == null)
      return false;

    if (!cell.RemoveTile(layer))
      return false;

    if (cell.IsEmpty)
      _registry.Remove(cellPos);

    _renderer.ClearTile(cellPos, layer);

    return true;
  }

  // -----------------------------
  // Object Placement
  // -----------------------------
  public bool TryPlaceObject(GameObject obj)
  {
    if (!obj.TryGetComponent<WorldObject>(out var worldObject))
      return false;

    float cellSize = GridConverter.CellSize;

    List<Vector3Int> placementCells = new();
    List<Vector3Int> obstacleCells = new();

    // -------------------------
    // 1️⃣ Collect placement cells
    // -------------------------
    foreach (var (bottomLeft, size) in worldObject.GetPlacementFootprint(cellSize))
    {
      Vector3Int baseCell = GridConverter.WorldToCell(bottomLeft);

      for (int x = 0; x < size.x; x++)
      {
        for (int y = 0; y < size.y; y++)
        {
          Vector3Int cell = new(baseCell.x + x, baseCell.y + y, 0);
          placementCells.Add(cell);
        }
      }
    }

    // -------------------------
    // 2️⃣ Collect obstacle cells
    // -------------------------
    foreach (var (bottomLeft, size) in worldObject.GetObstacleFootprints(cellSize))
    {
      Vector3Int baseCell = GridConverter.WorldToCell(bottomLeft);

      for (int x = 0; x < size.x; x++)
      {
        for (int y = 0; y < size.y; y++)
        {
          Vector3Int cell = new(baseCell.x + x, baseCell.y + y, 0);
          obstacleCells.Add(cell);
        }
      }
    }

    // -------------------------
    // 3️⃣ Validate placement
    // -------------------------
    foreach (var cellPos in placementCells)
    {
      var cell = GetOrCreateCell(cellPos);

      if (cell.Object != null)
        return false;
    }

    // -------------------------
    // 4️⃣ Commit placement
    // -------------------------
    foreach (var cellPos in placementCells)
    {
      var cell = GetOrCreateCell(cellPos);

      cell.PlaceObject(obj, CellObjectFlags.Placement);
    }

    // -------------------------
    // 5️⃣ Commit obstacle
    // -------------------------
    foreach (var cellPos in obstacleCells)
    {
      var cell = GetOrCreateCell(cellPos);

      if (cell.Object == obj)
      {
        // merge flags
        cell.AddObjectFlag(CellObjectFlags.Obstacle);
      }
      else
      {
        cell.PlaceObject(obj, CellObjectFlags.Obstacle);
      }
    }

    // store placement cells for removal
    _objectCells[obj] = placementCells;

    return true;
  }

  public void RemoveObject(GameObject obj)
  {
    if (!_objectCells.TryGetValue(obj, out var cells))
      return;

    var adjacentFenceRules = obj.TryGetComponent<IFenceUpdatable>(out _)
        ? CollectAdjacentFenceRules(cells)
        : null;

    foreach (var cellPos in cells)
    {
      var cell = _registry.Get(cellPos);
      if (cell != null)
      {
        cell.RemoveObject();

        if (cell.IsEmpty)
          _registry.Remove(cellPos);
      }
    }

    _objectCells.Remove(obj);

    if (adjacentFenceRules != null)
      foreach (var rule in adjacentFenceRules)
        rule.UpdateBitmask();
  }

  private static readonly Vector3Int[] _fenceDirs =
  {
    Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left,
  };

  private List<IFenceUpdatable> CollectAdjacentFenceRules(List<Vector3Int> placedCells)
  {
    var rules = new List<IFenceUpdatable>();
    var visited = new HashSet<Vector3Int>();

    foreach (var cellPos in placedCells)
    {
      foreach (var dir in _fenceDirs)
      {
        var neighborPos = cellPos + dir;
        if (!visited.Add(neighborPos)) continue;

        var cell = _registry.Get(neighborPos);
        if (cell != null &&
            cell.Object != null &&
            cell.Object.TryGetComponent<IFenceUpdatable>(out var updatable))
          rules.Add(updatable);
      }
    }

    return rules;
  }

  // -----------------------------
  // Utility
  // -----------------------------

  public IReadOnlyList<WorldCell> GetCellsInRadius(
    Vector2 worldCenter,
    float radius)
    => _spatialQuery.GetCellsInRadius(worldCenter, radius);

  public IReadOnlyList<WorldCell> GetCellsAlongLine(
    Vector2 origin,
    Vector2 dir,
    float length)
    => _spatialQuery.GetCellsAlongLine(origin, dir, length);

  public IReadOnlyList<WorldCell> GetCellsInLine(
    Vector2 origin,
    Vector2 dir,
    float length,
    float width)
    => _spatialQuery.GetCellsInLine(origin, dir, length, width);

  public IReadOnlyList<WorldCell> GetCellsFromArea(
    Vector2 origin,
    Vector2 size)
    => _spatialQuery.GetCellsFromArea(origin, size);
}