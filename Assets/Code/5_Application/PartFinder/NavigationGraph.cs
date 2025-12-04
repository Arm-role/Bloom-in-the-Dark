using System.Collections.Generic;
using UnityEngine;

public class NavigationGraph
{
    public struct Node
    {
        public Vector3Int TilePos;
        public Vector3 WorldCenter;
        public List<int> Neighbors;
    }

    private readonly Dictionary<Vector3Int, int> _tileToIndex = new();
    private readonly List<Node> _nodes = new();

    public int NodeCount => _nodes.Count;

    public bool TryGetNodeIndex(Vector3Int tile, out int index) => _tileToIndex.TryGetValue(tile, out index);
    public bool GraphContainsNode(Vector3Int tile) => _tileToIndex.ContainsKey(tile);

    public Node GetNode(int index) => _nodes[index];

    public void AddNode(Vector3Int tile, Vector3 worldCenter)
    {
        if (_tileToIndex.ContainsKey(tile)) return;
        int idx = _nodes.Count;
        _tileToIndex.Add(tile, idx);
        _nodes.Add(new Node { TilePos = tile, WorldCenter = worldCenter, Neighbors = new List<int>() });
    }

    public void BuildAdjacency()
    {
        Vector3Int[] dirs = { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) };
        for (int i = 0; i < _nodes.Count; i++)
        {
            var node = _nodes[i];
            node.Neighbors.Clear();
            foreach (var d in dirs)
            {
                var nb = node.TilePos + d;
                if (_tileToIndex.TryGetValue(nb, out int idx)) node.Neighbors.Add(idx);
            }
            _nodes[i] = node;
        }
    }
}