#nullable enable
using System.Collections.Generic;
using UnityEngine;

// Facade over the world-grid subsystem. Owns the cell registry and the shared
// cell-creation policy, and delegates storage, queries, tile edits, object
// placement, and zone application to focused collaborators.
public sealed class WorldTileManager : MonoBehaviour
{
  private readonly WorldCellRegistry _registry = new();

  private ICellActionResolver _actionResolver = null!;
  private GridSpatialQuery _spatialQuery = null!;
  private WorldZoneApplier _zoneApplier = null!;
  private TileModificationService _modification = null!;
  private WorldObjectPlacementService _placement = null!;

  public IGridConverter GridConverter { get; private set; } = null!;

  public IEnumerable<GameObject> Objects => _placement.Objects;

  public void Initialize(
    List<TilemapLayer> tilemapLayers,
    ITileLibrary tileLibrary,
    IGridConverter gridConverter,
    ICellActionResolver actionResolver,
    WorldZoneManager zoneManager,
    IReadOnlyList<WorldObject> worldObjects)
  {
    GridConverter = gridConverter;
    _actionResolver = actionResolver;

    _spatialQuery = new GridSpatialQuery(_registry.Cells, GridConverter);
    _zoneApplier = new WorldZoneApplier(zoneManager, _registry, _spatialQuery);

    var renderer = new TileLayerRenderer(tilemapLayers);
    _modification = new TileModificationService(_registry, renderer, GetOrCreateCell);
    _placement = new WorldObjectPlacementService(_registry, GridConverter, GetOrCreateCell);

    _registry.Clear();

    var scanner = new TileLayerScanner(tileLibrary, GetOrCreateCell);
    foreach (var layer in tilemapLayers)
    {
      if (layer.tilemap == null) continue;
      scanner.Scan(layer.layerType, layer.tilemap);
    }

    _placement.RegisterMapObjects(worldObjects);
  }

  private void OnDestroy() => _zoneApplier?.Dispose();

  // Shared cell-creation policy: new cells receive current zone flags.
  private WorldCell GetOrCreateCell(Vector3Int cellPos)
  {
    var existing = _registry.Get(cellPos);
    if (existing != null)
      return existing;

    var cell = new WorldCell(
      cellPos,
      GridConverter.GetCellCenterWorld(cellPos),
      _actionResolver);

    _zoneApplier.RefreshCell(cell);

    _registry.Add(cellPos, cell);
    return cell;
  }

  // --- Cell lookup ---

  public WorldCell? GetCell(Vector3Int cellPos)
    => _registry.Get(cellPos);

  public WorldCell? GetCellFromWorld(Vector3 worldPos)
    => _registry.Get(GridConverter.WorldToCell(worldPos));

  public IEnumerable<WorldCell> GetAllCells()
    => _registry.All;

  // --- Tile modification ---

  public bool TryAddTile(Vector3Int cellPos, ETileLayerType layer, IBaseTileData tileData)
    => _modification.TryAddTile(cellPos, layer, tileData);

  public bool TryRemoveTile(Vector3Int cellPos, ETileLayerType layer)
    => _modification.TryRemoveTile(cellPos, layer);

  // --- Object placement ---

  public bool TryPlaceObject(GameObject obj)
    => _placement.TryPlaceObject(obj);

  public void RemoveObject(GameObject obj)
    => _placement.RemoveObject(obj);

  // --- Zone application ---

  [ContextMenu("Refresh All Zones")]
  public void RefreshAllZones() => _zoneApplier?.RefreshAll();

  public void ApplyZone(IWorldZone zone) => _zoneApplier?.ApplyZone(zone);

  // --- Spatial queries ---

  public IReadOnlyList<WorldCell> GetCellsInRadius(Vector2 worldCenter, float radius)
    => _spatialQuery.GetCellsInRadius(worldCenter, radius);

  public IReadOnlyList<WorldCell> GetCellsAlongLine(Vector2 origin, Vector2 dir, float length)
    => _spatialQuery.GetCellsAlongLine(origin, dir, length);

  public IReadOnlyList<WorldCell> GetCellsInLine(
    Vector2 origin, Vector2 dir, float length, float width)
    => _spatialQuery.GetCellsInLine(origin, dir, length, width);

  public IReadOnlyList<WorldCell> GetCellsFromArea(Vector2 origin, Vector2 size)
    => _spatialQuery.GetCellsFromArea(origin, size);
}
