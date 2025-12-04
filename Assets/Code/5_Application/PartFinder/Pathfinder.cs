using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private readonly NavigationGraph _graph;
    public Pathfinder(NavigationGraph graph) { _graph = graph ?? throw new ArgumentNullException(nameof(graph)); }

    private struct HeapItem : IComparable<HeapItem>
    {
        public float F; public int Index;
        public int CompareTo(HeapItem other) => F.CompareTo(other.F);
        int IComparable<HeapItem>.CompareTo(HeapItem other) => F.CompareTo(other.F);
        public int CompareTo(object obj) => F.CompareTo(((HeapItem)obj).F);
    }

    private class MinHeap
    {
        private List<HeapItem> _data = new List<HeapItem>();
        public int Count => _data.Count;
        public void Clear() => _data.Clear();
        public void Push(HeapItem item)
        {
            _data.Add(item);
            int i = _data.Count - 1;
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (_data[p].F <= _data[i].F) break;
                var tmp = _data[p]; _data[p] = _data[i]; _data[i] = tmp; i = p;
            }
        }
        public HeapItem Pop()
        {
            var root = _data[0];
            int last = _data.Count - 1;
            _data[0] = _data[last]; _data.RemoveAt(last);
            int i = 0;
            while (true)
            {
                int l = 2 * i + 1; int r = l + 1;
                if (l >= _data.Count) break;
                int smallest = l;
                if (r < _data.Count && _data[r].F < _data[l].F) smallest = r;
                if (_data[i].F <= _data[smallest].F) break;
                var tmp = _data[i]; _data[i] = _data[smallest]; _data[smallest] = tmp;
                i = smallest;
            }
            return root;
        }
    }

    public List<Vector3> FindPath(Vector3Int startTile, Vector3Int goalTile)
    {
        if (!_graph.TryGetNodeIndex(startTile, out int startIdx)) return null;
        if (!_graph.TryGetNodeIndex(goalTile, out int goalIdx)) return null;

        var open = new MinHeap();
        var bestG = new Dictionary<int, float>(Mathf.Max(16, _graph.NodeCount / 8));
        var parent = new Dictionary<int, int>();

        bestG[startIdx] = 0f;
        float h0 = Heuristic(_graph.GetNode(startIdx).TilePos, _graph.GetNode(goalIdx).TilePos);
        open.Push(new HeapItem { F = h0, Index = startIdx });

        int iterations = 0;
        int maxIter = _graph.NodeCount * 4;

        while (open.Count > 0)
        {
            var top = open.Pop();
            int cur = top.Index;
            if (!bestG.TryGetValue(cur, out float curG)) continue;
            if (cur == goalIdx) return Reconstruct(parent, startIdx, goalIdx);

            var node = _graph.GetNode(cur);
            foreach (var nb in node.Neighbors)
            {
                float tg = curG + 1f;
                if (!bestG.TryGetValue(nb, out float kg) || tg < kg)
                {
                    bestG[nb] = tg;
                    parent[nb] = cur;
                    float f = tg + Heuristic(_graph.GetNode(nb).TilePos, _graph.GetNode(goalIdx).TilePos);
                    open.Push(new HeapItem { F = f, Index = nb });
                }
            }

            iterations++;
            if (iterations > maxIter)
            {
                Debug.LogWarning("Pathfinder: exceeded max iterations.");
                break;
            }
        }
        return null;
    }

    private float Heuristic(Vector3Int a, Vector3Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    private List<Vector3> Reconstruct(Dictionary<int, int> parent, int startIdx, int goalIdx)
    {
        List<Vector3> rev = new List<Vector3>();
        int cur = goalIdx;
        rev.Add(_graph.GetNode(cur).WorldCenter);
        while (cur != startIdx)
        {
            if (!parent.TryGetValue(cur, out int p)) return null;
            cur = p;
            rev.Add(_graph.GetNode(cur).WorldCenter);
        }
        rev.Reverse();
        return rev;
    }
}
