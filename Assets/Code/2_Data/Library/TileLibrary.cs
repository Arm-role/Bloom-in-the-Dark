using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileLibrary", menuName = "Library/TileLibrary")]
public class TileLibrary : ScriptableObject
{
    [SerializeField] private List<TileBaseData> _tiles = new();
    private Dictionary<TileBase, TileBaseData> _tileLookup;

    public void Initialize()
    {
        _tileLookup = new Dictionary<TileBase, TileBaseData>();
        foreach (var t in _tiles)
        {
            if (t.Tile != null && !_tileLookup.ContainsKey(t.Tile))
                _tileLookup.Add(t.Tile, t);
        }
    }

    public TileBaseData GetTileData(TileBase tile)
    {
        if (_tileLookup == null) Initialize();
        _tileLookup.TryGetValue(tile, out TileBaseData tileData);
        return tileData;
    }

    public TileBaseData GetTileDataByName(string tileName)
    {
        if (_tileLookup == null) Initialize();

        foreach (var tileData in _tileLookup.Values)
        {
            if (tileData.DisplayName == tileName)
            {
                return tileData;
            }
        }
        return null;
    }

    public void ClearTiles() => _tiles.Clear();

}
