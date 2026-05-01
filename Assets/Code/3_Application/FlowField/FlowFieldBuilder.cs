using System.Collections.Generic;
using UnityEngine;

public static class FlowFieldBuilder
{
  private static readonly Vector2Int[] NEI = new Vector2Int[]
  {
        new Vector2Int(1,0),  new Vector2Int(-1,0),
        new Vector2Int(0,1),  new Vector2Int(0,-1),
        new Vector2Int(1,1),  new Vector2Int(1,-1),
        new Vector2Int(-1,1), new Vector2Int(-1,-1),
  };

  private const int INF = int.MaxValue / 4;

  public static void BuildFromTargets(FlowField field, List<Vector2Int> targets)
  {
    for (int x = 0; x < field.width; x++)
      for (int y = 0; y < field.height; y++)
        field.integrationField[x, y] = INF;

    Queue<Vector2Int> q = new();

    foreach (var t in targets)
    {
      if (!field.IsInside(t)) continue;
      if (field.GetCost(t) == FlowField.COST_IMPASSABLE) continue;

      field.SetIntegration(t, 0);
      q.Enqueue(t);
    }

    while (q.Count > 0)
    {
      var cur = q.Dequeue();
      int curVal = field.GetIntegration(cur);

      foreach (var off in NEI)
      {
        var n = cur + off;
        if (!field.IsInside(n)) continue;
        if (field.GetCost(n) == FlowField.COST_IMPASSABLE) continue;

        // BUG FIX #2: ส่ง cur ไม่ใช่ idx (ชื่อเดิมสับสน)
        if (IsDiagonalCut(field, cur, off)) continue;

        int moveCost = (off.x != 0 && off.y != 0)
            ? FlowField.COST_DIAGONAL
            : FlowField.COST_STRAIGHT;

        int newVal = curVal + moveCost + field.GetCost(n);

        if (newVal < field.GetIntegration(n))
        {
          field.SetIntegration(n, newVal);
          q.Enqueue(n);
        }
      }
    }

    for (int x = 0; x < field.width; x++)
      for (int y = 0; y < field.height; y++)
      {
        Vector2Int idx = new Vector2Int(x, y);
        field.SetDirection(idx, CalculateDirection(field, idx));
      }

    // BUG FIX #7: set ครั้งเดียวหลัง loop ไม่ใช่ทุก iteration
    field.IsBuilt = true;
  }

  // BUG FIX #2: edge-of-grid → ถ้า side อยู่นอก grid ให้ถือว่า block
  // เหตุผล: นอก grid = ไม่มีข้อมูล = ไม่ควรเดินผ่านมุมนั้น
  private static bool IsDiagonalCut(FlowField f, Vector2Int idx, Vector2Int off)
  {
    if (off.x == 0 || off.y == 0) return false;

    Vector2Int side1 = new(idx.x + off.x, idx.y);
    Vector2Int side2 = new(idx.x, idx.y + off.y);

    // BUG FIX: !IsInside → ถือว่า blocked (เดิม !IsInside → ถือว่า passable)
    bool block1 = !f.IsInside(side1) || f.GetCost(side1) == FlowField.COST_IMPASSABLE;
    bool block2 = !f.IsInside(side2) || f.GetCost(side2) == FlowField.COST_IMPASSABLE;

    return block1 || block2;
  }

  // BUG FIX #3: ลบ double-fallback ออก เหลือ loop เดียว
  // loop แรกเดิมทำงานเหมือน loop ที่สองทุกอย่าง เพราะ best = current
  // ถ้าไม่มี neighbor ที่ดีกว่า current → bestDir = zero → เข้า fallback
  // แต่ fallback ทำแบบเดิมทุกอย่าง → ซ้ำซ้อน 100%
  private static Vector2 CalculateDirection(FlowField f, Vector2Int idx)
  {
    if (f.GetIntegration(idx) >= INF)
      return Vector2.zero;

    int best = int.MaxValue;
    Vector2 bestDir = Vector2.zero;

    foreach (var off in NEI)
    {
      Vector2Int n = idx + off;
      if (!f.IsInside(n)) continue;
      if (IsDiagonalCut(f, idx, off)) continue;

      int v = f.GetIntegration(n);
      if (v < best)
      {
        best = v;
        bestDir = off;
      }
    }

    return bestDir == Vector2.zero ? Vector2.zero : bestDir.normalized;
  }
}