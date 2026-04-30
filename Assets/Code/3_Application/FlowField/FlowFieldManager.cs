using System;
using UnityEngine;
using System.Collections.Generic;

public class FlowFieldManager : MonoBehaviour
{
  public static FlowFieldManager Instance { get; private set; }

  [Header("References")]
  public WorldTileManager world;

  // autosized on first build
  private int gridWidth = 0;
  private int gridHeight = 0;

  private Vector3Int _minCell;
  private Vector3Int _maxCell;

  private Dictionary<FlowFieldKey, FlowField> _fields = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  // ---------------------------------------------------------
  // BUILD FIELD
  // ---------------------------------------------------------
  public FlowField BuildField(
    FlowFieldChannelKey channel,
    Vector2Int footprint,
    IEnumerable<Vector3> targets)
  {
    if (world == null)
      throw new Exception("FlowFieldManager requires WorldTileManager");

    EnsureBounds();

    FlowFieldKey key = new FlowFieldKey(channel, footprint);

    if (_fields.TryGetValue(key, out var existing))
      return existing;

    Vector3 originWorld = world.GridConverter.GetCellCenterWorld(_minCell);
    Vector3Int originCell = _minCell;
    int w = gridWidth;
    int h = gridHeight;

    FlowField f = new FlowField(w, h, originWorld, world.GridConverter.CellSize, originCell);

    // -------------------------------------------------
    // BUILD COST FIELD (footprint aware)
    // -------------------------------------------------

    for (int x = 0; x < w; x++)
    {
      for (int y = 0; y < h; y++)
      {
        Vector3Int cell = new Vector3Int(originCell.x + x, originCell.y + y, 0);
        var state = world.GetCell(cell);

        if (!IsCellPassableForFootprint(cell, footprint))
        {
          f.SetCost(new Vector2Int(x, y), FlowField.COST_IMPASSABLE);
          continue;
        }
        f.SetCost(new Vector2Int(x, y), FlowField.COST_STRAIGHT);
      }
    }

    // -------------------------------------------------
    // TARGET CELLS
    // -------------------------------------------------

    var targetCells = new List<Vector2Int>();
    foreach (var wp in targets)
    {
      Vector3Int wc = world.GridConverter.WorldToCell(wp);
      var rel = new Vector2Int(wc.x - originCell.x, wc.y - originCell.y);
      if (rel.x >= 0 && rel.x < w && rel.y >= 0 && rel.y < h)
        targetCells.Add(rel);
        targetCells.Add(rel);

      //Debug.Log($"Target world: {wp} -> cell: {wc} -> rel: {rel}");
    }


    FlowFieldBuilder.BuildFromTargets(f, targetCells);
    _fields[key] = f;
    return f;
  }

  public FlowField BuildField(FlowFieldChannelKey key, Vector2Int footprint, Vector3 singleTarget)
      => BuildField(key, footprint, new Vector3[] { singleTarget });


  // ---------------------------------------------------------
  // CLEARANCE CHECK
  // ---------------------------------------------------------

  // AFTER: เช็คว่า footprint ที่วางบน centerCell จะชนอะไรไหม
  // แต่ใช้ Minkowski sum erosion เฉพาะตอน build cost field
  public bool HasClearance(Vector3Int pivotCell, Vector2Int footprint, Vector2Int pivotOffset)
  {
    for (int dx = 0; dx < footprint.x; dx++)
      for (int dy = 0; dy < footprint.y; dy++)
      {
        var check = new Vector3Int(
            pivotCell.x + (dx - pivotOffset.x),
            pivotCell.y + (dy - pivotOffset.y),
            0);

        var state = world.GetCell(check);
        if (state == null || state.BlocksMovement) return false;
      }
    return true;
  }

  private bool IsCellPassableForFootprint(Vector3Int cell, Vector2Int footprint)
  {
    // เช็คว่า bottom-left corner ของ footprint วางที่ cell นี้แล้วชนไหม
    for (int dx = 0; dx < footprint.x; dx++)
      for (int dy = 0; dy < footprint.y; dy++)
      {
        var c = new Vector3Int(cell.x + dx, cell.y + dy, 0);
        var state = world.GetCell(c);
        if (state == null || state.BlocksMovement) return false;
      }
    return true;
  }

  // ---------------------------------------------------------
  // FIELD ACCESS
  // ---------------------------------------------------------

  public bool TryGetField(FlowFieldChannelKey channel, Vector2Int footprint, out FlowField flowField)
  {
    FlowFieldKey key = new FlowFieldKey(channel, footprint);
    return _fields.TryGetValue(key, out flowField);
  }

  public FlowField GetField(FlowFieldChannelKey channel, Vector2Int footprint)
  {
    FlowFieldKey key = new FlowFieldKey(channel, footprint);
    _fields.TryGetValue(key, out var f);

    return f;
  }

  public void RemoveField(FlowFieldChannelKey channel, Vector2Int footprint)
  {
    FlowFieldKey key = new FlowFieldKey(channel, footprint);
    _fields.Remove(key);
  }

  // ---------------------------------------------------------
  // GRID BOUNDS AUTO-DETECT
  // ---------------------------------------------------------

  private void EnsureBounds()
  {
    if (gridWidth != 0 && gridHeight != 0) return;

    int minX = int.MaxValue;
    int minY = int.MaxValue;

    int maxX = int.MinValue;
    int maxY = int.MinValue;

    int c = 0;

    foreach (var s in world.GetAllCells())
    {
      c++;
      minX = Mathf.Min(minX, s.CellPos.x);
      minY = Mathf.Min(minY, s.CellPos.y);
      maxX = Mathf.Max(maxX, s.CellPos.x);
      maxY = Mathf.Max(maxY, s.CellPos.y);
    }

    if (c == 0) throw new Exception("WorldTileManager has no tiles");

    _minCell = new Vector3Int(minX, minY, 0);
    _maxCell = new Vector3Int(maxX, maxY, 0);
    gridWidth = maxX - minX + 1;
    gridHeight = maxY - minY + 1;
  }

  public List<Vector3> FindClosestReachableCells(
  Vector3 targetWorld,
  Vector2Int footprint,
  Vector2Int pivotOffset,
  int maxSearchRadius = 12)
  {
    var grid = world.GridConverter;

    Vector3Int start =
      grid.WorldToCell(targetWorld);

    var visited =
      new HashSet<Vector3Int>();

    var frontier =
      new Queue<Vector3Int>();

    frontier.Enqueue(start);
    visited.Add(start);

    int depth = 0;

    while (frontier.Count > 0 &&
          depth < maxSearchRadius)
    {
      int layerSize =
        frontier.Count;

      var layerResults =
        new List<Vector3>();

      for (int i = 0; i < layerSize; i++)
      {
        var cell =
          frontier.Dequeue();

        if (HasClearance(cell, footprint, pivotOffset))
        {
          layerResults.Add(
            grid.CellToWorld(cell)
          );
        }

        Expand(cell);
      }

      if (layerResults.Count > 0)
        return layerResults;

      depth++;
    }

    return new List<Vector3>();


    void Expand(Vector3Int c)
    {
      Try(c + Vector3Int.up);
      Try(c + Vector3Int.down);
      Try(c + Vector3Int.left);
      Try(c + Vector3Int.right);
    }

    void Try(Vector3Int next)
    {
      if (visited.Contains(next))
        return;

      visited.Add(next);
      frontier.Enqueue(next);
    }
  }
}