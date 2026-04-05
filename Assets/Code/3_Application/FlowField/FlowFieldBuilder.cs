using System.Collections.Generic;
using UnityEngine;

public static class FlowFieldBuilder
{
  private static readonly Vector2Int[] NEI = new Vector2Int[]
  {
        new Vector2Int(1,0),   new Vector2Int(-1,0),
        new Vector2Int(0,1),   new Vector2Int(0,-1),

        new Vector2Int(1,1),   new Vector2Int(1,-1),
        new Vector2Int(-1,1),  new Vector2Int(-1,-1),
  };

  private const int INF = int.MaxValue / 4;

  public static void BuildFromTargets(FlowField field, List<Vector2Int> targets)
  {
    // init integration to INF
    for (int x = 0; x < field.width; x++)
      for (int y = 0; y < field.height; y++)
        field.integrationField[x, y] = INF;

    Queue<Vector2Int> q = new();

    // add all targets
    foreach (var t in targets)
    {
      if (!field.IsInside(t)) continue;
      if (field.GetCost(t) == FlowField.COST_IMPASSABLE) continue;

      field.SetIntegration(t, 0);
      q.Enqueue(t);
    }

    // BFS (Dijkstra style)
    while (q.Count > 0)
    {
      var cur = q.Dequeue();
      int curVal = field.GetIntegration(cur);

      foreach (var off in NEI)
      {
        var n = cur + off;
        if (!field.IsInside(n)) continue;

        // skip blocked tiles
        if (field.GetCost(n) == FlowField.COST_IMPASSABLE) continue;

        // skip illegal diagonal cut
        if (IsDiagonalCut(field, cur, off)) continue;

        int moveCost =
            (off.x != 0 && off.y != 0) ?
            FlowField.COST_DIAGONAL :
            FlowField.COST_STRAIGHT;

        int newVal = curVal + moveCost + field.GetCost(n);

        if (newVal < field.GetIntegration(n))
        {
          field.SetIntegration(n, newVal);
          q.Enqueue(n);
        }
      }
    }

    // -----------------------------
    // Calculate flow direction
    // -----------------------------
    for (int x = 0; x < field.width; x++)
    {
      for (int y = 0; y < field.height; y++)
      {
        Vector2Int idx = new Vector2Int(x, y);
        Vector2 dir = CalculateDirection(field, idx);
        field.SetDirection(idx, dir);
        field.IsBuilt = true;
      }
    }
  }

  // -----------------------------------------
  // TRUE diagonal rule for all modern pathfinding
  // -----------------------------------------
  private static bool IsDiagonalCut(FlowField f, Vector2Int idx, Vector2Int off)
  {
    // not diagonal => cannot cut
    if (off.x == 0 || off.y == 0) return false;

    // diagonal neighbors
    Vector2Int side1 = new(idx.x + off.x, idx.y);
    Vector2Int side2 = new(idx.x, idx.y + off.y);

    // If either side is blocked → cannot move diagonally
    bool block1 = f.IsInside(side1) && f.GetCost(side1) == FlowField.COST_IMPASSABLE;
    bool block2 = f.IsInside(side2) && f.GetCost(side2) == FlowField.COST_IMPASSABLE;

    return block1 || block2;
  }

  // -----------------------------------------
  // Choose lowest integration neighbor
  // -----------------------------------------
  private static Vector2 CalculateDirection(FlowField f, Vector2Int idx)
  {
    int current = f.GetIntegration(idx);
    if (current >= INF)
      return Vector2.zero;

    int best = current;
    Vector2 bestDir = Vector2.zero;

    foreach (var off in NEI)
    {
      Vector2Int n = idx + off;
      if (!f.IsInside(n)) continue;

      // เช็คไม่ให้เดิน diagonal ที่ติดมุม
      if (IsDiagonalAndBlocked(f, idx, off))
        continue;

      int v = f.GetIntegration(n);
      if (v < best)
      {
        best = v;
        bestDir = off;
      }
    }

    // fallback (แต่ต้องไม่ใช่ diagonal illegal)
    if (bestDir == Vector2.zero)
    {
      int lowest = INF;
      Vector2 fallback = Vector2.zero;

      foreach (var off in NEI)
      {
        Vector2Int n = idx + off;
        if (!f.IsInside(n)) continue;

        if (IsDiagonalAndBlocked(f, idx, off))
          continue;

        int v = f.GetIntegration(n);
        if (v < lowest && v < INF)
        {
          lowest = v;
          fallback = off;
        }
      }

      if (fallback != Vector2.zero)
        return fallback.normalized;
    }

    return bestDir.normalized;
  }

  private static bool IsDiagonalAndBlocked(FlowField f, Vector2Int idx, Vector2Int off)
  {
    if (off.x == 0 || off.y == 0)
      return false;

    Vector2Int side1 = new(idx.x + off.x, idx.y);
    Vector2Int side2 = new(idx.x, idx.y + off.y);

    bool b1 = f.IsInside(side1) && f.GetCost(side1) == FlowField.COST_IMPASSABLE;
    bool b2 = f.IsInside(side2) && f.GetCost(side2) == FlowField.COST_IMPASSABLE;

    return b1 || b2;
  }
}
