using System.Collections.Generic;
using UnityEngine;

public class SpatialHashGrid<T> where T : class
{
    private readonly Dictionary<int, List<T>> _buckets = new Dictionary<int, List<T>>();
    private readonly Dictionary<T, int> _currentBucket = new Dictionary<T, int>();
    private readonly float _cellSize;

    public SpatialHashGrid(float cellSize)
    {
        _cellSize = Mathf.Max(0.0001f, cellSize);
    }

    private int Hash(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x / _cellSize);
        int y = Mathf.FloorToInt(pos.y / _cellSize);
        unchecked
        {
            return x * 73856093 ^ y * 19349663;
        }
    }

    public void Insert(T item, Vector2 pos)
    {
        int h = Hash(pos);
        if (!_buckets.TryGetValue(h, out var list))
        {
            list = new List<T>();
            _buckets[h] = list;
        }
        list.Add(item);
        _currentBucket[item] = h;
    }

    public void Remove(T item)
    {
        if (!_currentBucket.TryGetValue(item, out var h)) return;
        if (_buckets.TryGetValue(h, out var list))
        {
            list.Remove(item);
            if (list.Count == 0) _buckets.Remove(h);
        }
        _currentBucket.Remove(item);
    }

    public void Update(T item, Vector2 newPos)
    {
        int newHash = Hash(newPos);
        if (_currentBucket.TryGetValue(item, out var oldHash))
        {
            if (oldHash == newHash) return;
            if (_buckets.TryGetValue(oldHash, out var oldList))
            {
                oldList.Remove(item);
                if (oldList.Count == 0) _buckets.Remove(oldHash);
            }
        }
        if (!_buckets.TryGetValue(newHash, out var list))
        {
            list = new List<T>();
            _buckets[newHash] = list;
        }
        list.Add(item);
        _currentBucket[item] = newHash;
    }

    /// <summary>
    /// Query neighbors within radius. resultList is cleared and filled.
    /// posGetter returns the current position of an item.
    /// </summary>
    public void Query(Vector2 center, float radius, List<T> resultList, System.Func<T, Vector2> posGetter)
    {
        resultList.Clear();
        int cx = Mathf.FloorToInt(center.x / _cellSize);
        int cy = Mathf.FloorToInt(center.y / _cellSize);
        int range = Mathf.CeilToInt(radius / _cellSize);

        float r2 = radius * radius;

        for (int dx = -range; dx <= range; ++dx)
        {
            for (int dy = -range; dy <= range; ++dy)
            {
                int x = cx + dx;
                int y = cy + dy;
                int h = unchecked(x * 73856093 ^ y * 19349663);
                if (!_buckets.TryGetValue(h, out var list)) continue;
                for (int i = 0; i < list.Count; ++i)
                {
                    var item = list[i];
                    var p = posGetter(item);
                    if ((p - center).sqrMagnitude <= r2)
                        resultList.Add(item);
                }
            }
        }
    }
}