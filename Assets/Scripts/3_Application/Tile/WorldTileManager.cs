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

  private WorldObjectPlacementService _placement;

  public IEnumerable<GameObject> Objects => _placement.Objects;

  // -----------------------------
  // Initialization
  // -----------------------------
  public void Initialize(
    List<TilemapLayer> tilemapLayers,
    ITileLibrary tileLibrary,
    IGridConverter gridConverter,
    ICellActionResolver actionResolver,
    WorldZoneManager zoneManager,
    IReadOnlyList<WorldObject> worldObjects)
  {
    _tileLibrary = tileLibrary;
    GridConverter = gridConverter;
    _actionResolver = actionResolver;
    _zoneManager = zoneManager;
    _renderer = new TileLayerRenderer(tilemapLayers);
    _spatialQuery = new GridSpatialQuery(_registry.Cells, GridConverter);
    _placement = new WorldObjectPlacementService(
      _registry, GridConverter, GetOrCreateCell);

    _registry.Clear();

    foreach (var layer in tilemapLayers)
    {
      if (layer.tilemap == null) continue;
      ScanTileLayer(layer.layerType, layer.tilemap);
    }

    _zoneManager.ZoneChanged += OnZoneChanged;

    _placement.RegisterMapObjects(worldObjects);
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
  // Object Placement (delegated to WorldObjectPlacementService)
  // -----------------------------
  public bool TryPlaceObject(GameObject obj)
    => _placement.TryPlaceObject(obj);

  public void RemoveObject(GameObject obj)
    => _placement.RemoveObject(obj);

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