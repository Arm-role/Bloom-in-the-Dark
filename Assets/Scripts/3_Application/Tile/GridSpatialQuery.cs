#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

// Read-only spatial lookups over the world cell registry.
// Plain C# (no MonoBehaviour) so it can be exercised by EditMode tests.
public sealed class GridSpatialQuery
{
  private readonly IReadOnlyDictionary<Vector3Int, WorldCell> _cells;
  private readonly IGridConverter _gridConverter;

  public GridSpatialQuery(
    IReadOnlyDictionary<Vector3Int, WorldCell> cells,
    IGridConverter gridConverter)
  {
    _cells = cells;
    _gridConverter = gridConverter;
  }

  public IReadOnlyList<WorldCell> GetCellsInRadius(
    Vector2 worldCenter,
    float radius)
  {
    List<WorldCell> result = new();
    float radiusSqr = radius * radius;

    Vector3Int minCell = _gridConverter.WorldToCell(
      worldCenter - Vector2.one * radius);
    Vector3Int maxCell = _gridConverter.WorldToCell(
      worldCenter + Vector2.one * radius);

    ForEachCellInBounds(minCell, maxCell, cell =>
    {
      float distSqr =
        (cell.WorldCenter - (Vector3)worldCenter).sqrMagnitude;

      if (distSqr <= radiusSqr)
        result.Add(cell);
    });

    return result;
  }

  public IReadOnlyList<WorldCell> GetCellsAlongLine(
    Vector2 origin,
    Vector2 dir,
    float length)
  {
    List<WorldCell> result = new();
    HashSet<Vector3Int> visited = new();

    dir.Normalize();

    float step = _gridConverter.CellSize * 0.5f;
    float traveled = 0f;

    while (traveled <= length)
    {
      Vector2 worldPos = origin + dir * traveled;
      Vector3Int cellPos = _gridConverter.WorldToCell(worldPos);

      if (visited.Add(cellPos) &&
          _cells.TryGetValue(cellPos, out var cell))
        result.Add(cell);

      traveled += step;
    }

    return result;
  }

  public IReadOnlyList<WorldCell> GetCellsInLine(
    Vector2 origin,
    Vector2 dir,
    float length,
    float width)
  {
    List<WorldCell> result = new();

    dir.Normalize();
    Vector2 right = new Vector2(-dir.y, dir.x);

    float halfWidth = width * 0.5f;
    float cellSize = _gridConverter.CellSize;

    Vector2 end = origin + dir * length;
    Vector2 min = Vector2.Min(origin, end) - Vector2.one * halfWidth;
    Vector2 max = Vector2.Max(origin, end) + Vector2.one * halfWidth;

    Vector3Int minCell = _gridConverter.WorldToCell(min);
    Vector3Int maxCell = _gridConverter.WorldToCell(max);

    ForEachCellInBounds(minCell, maxCell, cell =>
    {
      Vector2 toCell = (Vector2)cell.WorldCenter - origin;

      float forward = Vector2.Dot(toCell, dir);
      if (forward < 0f || forward > length)
        return;

      float side = Mathf.Abs(Vector2.Dot(toCell, right));
      if (side > halfWidth + cellSize * 0.5f)
        return;

      result.Add(cell);
    });

    return result;
  }

  public IReadOnlyList<WorldCell> GetCellsFromArea(
    Vector2 origin,
    Vector2 size)
  {
    List<WorldCell> result = new();

    Vector2 half = size * 0.5f;

    Vector3Int minCell = _gridConverter.WorldToCell(origin - half);
    Vector3Int maxCell = _gridConverter.WorldToCell(origin + half);

    ForEachCellInBounds(minCell, maxCell, result.Add);

    return result;
  }

  private void ForEachCellInBounds(
    Vector3Int minCell,
    Vector3Int maxCell,
    Action<WorldCell> visit)
  {
    for (int x = minCell.x; x <= maxCell.x; x++)
    {
      for (int y = minCell.y; y <= maxCell.y; y++)
      {
        Vector3Int cellPos = new(x, y, 0);

        if (_cells.TryGetValue(cellPos, out var cell))
          visit(cell);
      }
    }
  }
}
