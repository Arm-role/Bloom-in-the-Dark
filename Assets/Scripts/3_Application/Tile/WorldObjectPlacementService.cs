#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Owns world-object placement on the cell grid: which cells an object occupies,
// obstacle flags, and fence-rule refresh on removal. Plain C# — created and
// driven by WorldTileManager, which supplies the cell-creation policy.
public sealed class WorldObjectPlacementService
{
  private static readonly Vector3Int[] FenceDirs =
  {
    Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left,
  };

  private readonly WorldCellRegistry _registry;
  private readonly IGridConverter _gridConverter;
  private readonly Func<Vector3Int, WorldCell> _getOrCreateCell;

  private readonly Dictionary<GameObject, List<Vector3Int>> _objectCells = new();

  public WorldObjectPlacementService(
    WorldCellRegistry registry,
    IGridConverter gridConverter,
    Func<Vector3Int, WorldCell> getOrCreateCell)
  {
    _registry = registry;
    _gridConverter = gridConverter;
    _getOrCreateCell = getOrCreateCell;
  }

  public IEnumerable<GameObject> Objects => _objectCells.Keys;

  public void RegisterMapObjects(IReadOnlyList<WorldObject> worldObjects)
  {
    foreach (var obj in _objectCells.Keys.ToList())
      RemoveObject(obj);

    _objectCells.Clear();

    foreach (var worldObject in worldObjects)
      TryPlaceObject(worldObject.gameObject);

    TileDomainEvents.ObstacleScanCompleted();
  }

  public bool TryPlaceObject(GameObject obj)
  {
    if (!obj.TryGetComponent<WorldObject>(out var worldObject))
      return false;

    float cellSize = _gridConverter.CellSize;

    List<Vector3Int> placementCells =
      ExpandFootprint(worldObject.GetPlacementFootprint(cellSize));
    List<Vector3Int> obstacleCells =
      ExpandFootprint(worldObject.GetObstacleFootprints(cellSize));

    foreach (var cellPos in placementCells)
    {
      if (_getOrCreateCell(cellPos).Object != null)
        return false;
    }

    foreach (var cellPos in placementCells)
      _getOrCreateCell(cellPos).PlaceObject(obj, CellObjectFlags.Placement);

    foreach (var cellPos in obstacleCells)
    {
      var cell = _getOrCreateCell(cellPos);

      if (cell.Object == obj)
        cell.AddObjectFlag(CellObjectFlags.Obstacle);
      else
        cell.PlaceObject(obj, CellObjectFlags.Obstacle);
    }

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

  // Both placement and obstacle footprints expand a (bottomLeft, size) list
  // into the individual grid cells they cover.
  private List<Vector3Int> ExpandFootprint(
    IEnumerable<(Vector3 bottomLeft, Vector2Int size)> footprints)
  {
    var cells = new List<Vector3Int>();

    foreach (var (bottomLeft, size) in footprints)
    {
      Vector3Int baseCell = _gridConverter.WorldToCell(bottomLeft);

      for (int x = 0; x < size.x; x++)
        for (int y = 0; y < size.y; y++)
          cells.Add(new Vector3Int(baseCell.x + x, baseCell.y + y, 0));
    }

    return cells;
  }

  private List<IFenceUpdatable> CollectAdjacentFenceRules(List<Vector3Int> placedCells)
  {
    var rules = new List<IFenceUpdatable>();
    var visited = new HashSet<Vector3Int>();

    foreach (var cellPos in placedCells)
    {
      foreach (var dir in FenceDirs)
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
}
