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
        // init integration
        for (int x = 0; x < field.width; x++)
            for (int y = 0; y < field.height; y++)
                field.integrationField[x, y] = INF;

        Queue<Vector2Int> q = new();

        // push targets
        foreach (var t in targets)
        {
            if (!field.IsInside(t)) continue;
            if (field.GetCost(t) == FlowField.COST_IMPASSABLE) continue;

            field.SetIntegration(t, 0);
            q.Enqueue(t);
        }

        // BFS with weights
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            int curVal = field.GetIntegration(cur);

            foreach (var off in NEI)
            {
                var n = cur + off;
                if (!field.IsInside(n)) continue;

                if (field.GetCost(n) == FlowField.COST_IMPASSABLE) continue;

                // corner-cut prevention
                if (IsDiagonalCut(field, cur, off)) continue;

                int moveCost =
                    (off.x != 0 && off.y != 0)
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

        // Flow vector calculation
        for (int x = 0; x < field.width; x++)
        {
            for (int y = 0; y < field.height; y++)
            {
                Vector2Int idx = new Vector2Int(x, y);
                field.SetDirection(idx, CalculateDirection(field, idx));
            }
        }
    }

    // -------------------------------------------------------
    // Prevent diagonal moving through corners
    // -------------------------------------------------------
    private static bool IsDiagonalCut(FlowField f, Vector2Int idx, Vector2Int off)
    {
        if (off.x == 0 || off.y == 0) return false;

        Vector2Int n1 = new(idx.x + off.x, idx.y);
        Vector2Int n2 = new(idx.x, idx.y + off.y);

        bool block1 = f.IsInside(n1) && f.GetCost(n1) == FlowField.COST_IMPASSABLE;
        bool block2 = f.IsInside(n2) && f.GetCost(n2) == FlowField.COST_IMPASSABLE;

        return block1 || block2;
    }

    // -------------------------------------------------------
    // Choose lowest-cost neighbor among 8 directions
    // -------------------------------------------------------
    private static Vector2 CalculateDirection(FlowField f, Vector2Int idx)
    {
        int best = f.GetIntegration(idx);
        if (best == INF) return Vector2.zero;

        Vector2 bestDir = Vector2.zero;

        foreach (var off in NEI)
        {
            Vector2Int n = idx + off;
            if (!f.IsInside(n)) continue;

            int v = f.GetIntegration(n);
            if (v < best)
            {
                best = v;
                bestDir = off;
            }
        }

        return bestDir.normalized;
    }
}
