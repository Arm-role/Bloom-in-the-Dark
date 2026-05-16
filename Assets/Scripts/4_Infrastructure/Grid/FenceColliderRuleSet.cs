using System.Collections.Generic;
using UnityEngine;

// bit0=North  bit1=East  bit2=South  bit3=West  (matches FenceRule.cs)
// _fullPath = cross polygon (NSEW) วาดตามเข็มหรือทวนก็ได้ — script จัดการเอง
// Algorithm: หา 4 concave vertex = inner corner แล้ว clip arm ที่ inactive ออกสำหรับแต่ละ mask
[CreateAssetMenu(menuName = "Grid/FenceColliderRuleSet")]
public class FenceColliderRuleSet : ScriptableObject, IFenceColliderRuleSet
{
    [SerializeField] private Vector2[] _fullPath;

    private Vector2[][] _cache;

    private void OnEnable() => Rebuild();
    private void OnValidate() => Rebuild();

    private void Rebuild()
    {
        _cache = new Vector2[16][];
        for (int i = 0; i < 16; i++) _cache[i] = System.Array.Empty<Vector2>();

        if (_fullPath == null || _fullPath.Length < 8) return;

        var poly = EnsureClockwise(_fullPath);

        var concaveIdx = FindConcaveIndices(poly);
        if (concaveIdx.Count != 4) return;

        var centroid = Vector2.zero;
        foreach (var p in poly) centroid += p;
        centroid /= poly.Length;

        // จัด inner corner ตาม quadrant รอบ centroid
        int idxNW = -1, idxNE = -1, idxSE = -1, idxSW = -1;
        foreach (int i in concaveIdx)
        {
            var p = poly[i] - centroid;
            if      (p.x >= 0 && p.y >= 0) idxNE = i;
            else if (p.x >= 0)              idxSE = i;
            else if (p.y < 0)               idxSW = i;
            else                            idxNW = i;
        }

        if (idxNW < 0 || idxNE < 0 || idxSE < 0 || idxSW < 0) return;

        // Clockwise: N(NW→NE)=bit0, E(NE→SE)=bit1, S(SE→SW)=bit2, W(SW→NW)=bit3
        int[] starts = { idxNW, idxNE, idxSE, idxSW };
        int[] ends   = { idxNE, idxSE, idxSW, idxNW };
        int n = poly.Length;

        for (int mask = 0; mask < 16; mask++)
        {
            var pts = new List<Vector2>();
            for (int arm = 0; arm < 4; arm++)
            {
                pts.Add(poly[starts[arm]]);
                if ((mask >> arm & 1) == 1)
                {
                    // arm active: ใส่ทุก point ระหว่าง start→end
                    int idx = (starts[arm] + 1) % n;
                    while (idx != ends[arm])
                    {
                        pts.Add(poly[idx]);
                        idx = (idx + 1) % n;
                    }
                }
                // arm inactive: ข้ามตรงไป end (ปิด arm ด้วย edge ตรง)
            }
            _cache[mask] = pts.ToArray();
        }
    }

    // shoelace: area<0 = clockwise (Y-up), ถ้า CCW ให้ reverse
    private static Vector2[] EnsureClockwise(Vector2[] poly)
    {
        float area = 0f;
        int n = poly.Length;
        for (int i = 0; i < n; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % n];
            area += a.x * b.y - b.x * a.y;
        }
        if (area > 0f)
        {
            var rev = new Vector2[n];
            for (int i = 0; i < n; i++) rev[i] = poly[n - 1 - i];
            return rev;
        }
        return poly;
    }

    // cross product > 0 บน clockwise polygon = concave vertex (inner corner)
    private static List<int> FindConcaveIndices(Vector2[] poly)
    {
        var result = new List<int>();
        int n = poly.Length;
        for (int i = 0; i < n; i++)
        {
            var a = poly[(i - 1 + n) % n];
            var b = poly[i];
            var c = poly[(i + 1) % n];
            float cross = (b.x - a.x) * (c.y - b.y) - (b.y - a.y) * (c.x - b.x);
            if (cross > 1e-6f) result.Add(i);
        }
        return result;
    }

    public Vector2[] GetPath(int bitmask)
    {
        if (_cache == null) Rebuild();
        if ((uint)bitmask >= 16) return null;
        return _cache[bitmask];
    }
}
