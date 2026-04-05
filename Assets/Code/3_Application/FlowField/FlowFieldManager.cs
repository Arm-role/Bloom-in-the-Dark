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

  private Dictionary<FlowFieldChannelKey, FlowField> _fields = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  // ---------------------------------------------------------
  // BUILD FIELD
  // ---------------------------------------------------------
  public FlowField BuildField(FlowFieldChannelKey key, IEnumerable<Vector3> targets)
  {
    if (world == null) throw new Exception("FlowFieldManager requires WorldTileManager");
    EnsureBounds();

    Vector3 originWorld = world.GridConverter.CellToWorld(_minCell);
    Vector3Int originCell = _minCell;
    int w = gridWidth;
    int h = gridHeight;

    FlowField f = new FlowField(w, h, originWorld, world.GridConverter.CellSize, originCell);

    for (int x = 0; x < w; x++)
    {
      for (int y = 0; y < h; y++)
      {
        Vector3Int cell = new Vector3Int(originCell.x + x, originCell.y + y, 0);
        var state = world.GetCell(cell);

        if (state != null && state.BlocksMovement)
        {
          f.SetCost(new Vector2Int(x, y), FlowField.COST_IMPASSABLE);
          continue;
        }

        f.SetCost(new Vector2Int(x, y), FlowField.COST_STRAIGHT);
      }
    }

    var targetCells = new List<Vector2Int>();
    foreach (var wp in targets)
    {
      Vector3Int wc = world.GridConverter.WorldToCell(wp);
      var rel = new Vector2Int(wc.x - originCell.x, wc.y - originCell.y);
      if (rel.x >= 0 && rel.x < w && rel.y >= 0 && rel.y < h)
        targetCells.Add(rel);

      //Debug.Log($"Target world: {wp} -> cell: {wc} -> rel: {rel}");
    }


    FlowFieldBuilder.BuildFromTargets(f, targetCells);
    _fields[key] = f;
    return f;
  }

  public FlowField BuildField(FlowFieldChannelKey key, Vector3 singleTarget)
      => BuildField(key, new Vector3[] { singleTarget });

  public bool TryGetField(FlowFieldChannelKey key, out FlowField flowField)
  {
    flowField = null;
    if (key == null) return false;

    if (!_fields.TryGetValue(key, out flowField))
      return false;

    return true;
  }

  public Vector2 GetDirection(FlowFieldChannelKey key, Vector3Int worldCell)
  {
    if (!_fields.TryGetValue(key, out var f)) return Vector2.zero;

    Vector2Int idx = new Vector2Int(worldCell.x - f.originCell.x, worldCell.y - f.originCell.y);
    if (!f.IsInside(idx)) return Vector2.zero;

    return f.GetDirection(idx);
  }

  public void RemoveField(FlowFieldChannelKey key) => _fields.Remove(key);

  private void EnsureBounds()
  {
    if (gridWidth != 0 && gridHeight != 0) return;

    int minX = int.MaxValue, minY = int.MaxValue;
    int maxX = int.MinValue, maxY = int.MinValue;
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

  public FlowField GetField(FlowFieldChannelKey key)
  {
    if (key == null) return null;
    _fields.TryGetValue(key, out var f);
    return f;
  }

  public Vector2 GetDirectionLocal(FlowFieldChannelKey key, Vector2Int idx)
  {
    if (!_fields.TryGetValue(key, out var f))
      return Vector2.zero;

    if (!f.IsInside(idx))
      return Vector2.zero;

    return f.GetDirection(idx);
  }
}