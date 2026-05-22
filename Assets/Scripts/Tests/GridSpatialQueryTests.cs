#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// EditMode tests for GridSpatialQuery — verifies the spatial lookups extracted
// from WorldTileManager in Phase 1 behave as expected.
public sealed class GridSpatialQueryTests
{
  // Unit-size grid: cell (x,y) spans world [x,x+1) and is centred at (x+0.5,y+0.5).
  private sealed class FakeGridConverter : IGridConverter
  {
    public float CellSize => 1f;

    public Vector2Int WorldToGrid(Vector3 p)
      => new(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));

    public Vector3 GridToWorld(Vector2Int g) => new(g.x, g.y, 0f);

    public Vector3Int WorldToCell(Vector3 p)
      => new(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), 0);

    public Vector3 CellToWorld(Vector3Int c) => new(c.x, c.y, 0f);

    public Vector3 GetCellCenterWorld(Vector3Int c)
      => new(c.x + 0.5f, c.y + 0.5f, 0f);
  }

  private static GridSpatialQuery BuildQuery(params Vector3Int[] positions)
  {
    var converter = new FakeGridConverter();
    var cells = new Dictionary<Vector3Int, WorldCell>();

    foreach (var pos in positions)
      cells[pos] = new WorldCell(pos, converter.GetCellCenterWorld(pos), null);

    return new GridSpatialQuery(cells, converter);
  }

  private static HashSet<Vector3Int> Positions(IReadOnlyList<WorldCell> cells)
  {
    var set = new HashSet<Vector3Int>();
    foreach (var cell in cells)
      set.Add(cell.CellPos);
    return set;
  }

  [Test]
  public void GetCellsInRadius_ReturnsOnlyCellsWithinDistance()
  {
    var query = BuildQuery(
      new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 1, 0),
      new Vector3Int(1, 1, 0));

    var result = query.GetCellsInRadius(new Vector2(0.5f, 0.5f), radius: 1f);

    // (1,1) is sqrt(2) away — outside; (2,0) is 2 away — outside.
    CollectionAssert.AreEquivalent(
      new[]
      {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
      },
      Positions(result));
  }

  [Test]
  public void GetCellsInRadius_EmptyRegistry_ReturnsEmpty()
  {
    var query = BuildQuery();

    var result = query.GetCellsInRadius(new Vector2(0.5f, 0.5f), radius: 5f);

    Assert.IsEmpty(result);
  }

  [Test]
  public void GetCellsFromArea_ReturnsEveryCellWithinBounds()
  {
    var query = BuildQuery(
      new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 1, 0),
      new Vector3Int(1, 1, 0));

    // 2x2 box centred at (0.5,0.5) → cell bounds (-1,-1)..(1,1).
    var result = query.GetCellsFromArea(
      new Vector2(0.5f, 0.5f), new Vector2(2f, 2f));

    CollectionAssert.AreEquivalent(
      new[]
      {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
      },
      Positions(result));
  }

  [Test]
  public void GetCellsInLine_ReturnsCellsAlongHorizontalLine()
  {
    var query = BuildQuery(
      new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 1, 0),
      new Vector3Int(1, 1, 0));

    var result = query.GetCellsInLine(
      origin: new Vector2(0.5f, 0.5f),
      dir: new Vector2(1f, 0f),
      length: 2f,
      width: 0.5f);

    CollectionAssert.AreEquivalent(
      new[]
      {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(2, 0, 0),
      },
      Positions(result));
  }

  [Test]
  public void GetCellsAlongLine_CollectsUniqueCellsAlongRay()
  {
    var query = BuildQuery(
      new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 1, 0));

    var result = query.GetCellsAlongLine(
      origin: new Vector2(0.5f, 0.5f),
      dir: new Vector2(1f, 0f),
      length: 2f);

    CollectionAssert.AreEquivalent(
      new[]
      {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(2, 0, 0),
      },
      Positions(result));
  }
}
