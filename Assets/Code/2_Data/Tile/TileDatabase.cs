using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile/TileDatabase")]
public class TileDatabase : ScriptableObject
{
    [SerializeField] private List<TileDataEntry> entries = new();
    private Dictionary<TileBase, TileBaseData> _cache;

    [System.Serializable]
    public class TileDataEntry
    {
        public TileBase tile;
        public TileBaseData data;
    }

    public TileBaseData GetTileData(TileBase tile)
    {
        EnsureCache();
        _cache.TryGetValue(tile, out var data);
        return data;
    }

    public void RegisterTileData(TileBase tile, TileBaseData data)
    {
        EnsureCache();
        if (!_cache.ContainsKey(tile))
        {
            _cache[tile] = data;
            entries.Add(new TileDataEntry { tile = tile, data = data });
        }
    }

    private void EnsureCache()
    {
        if (_cache != null) return;
        _cache = new Dictionary<TileBase, TileBaseData>();
        foreach (var e in entries)
        {
            if (e.tile != null && e.data != null)
                _cache[e.tile] = e.data;
        }
    }
}