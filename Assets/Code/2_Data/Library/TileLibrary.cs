using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileLibrary", menuName = "Library/TileLibrary")]
public class TileLibrary : ScriptableObject
{
    [SerializeField] private BaseTileData[] allTiles;

    private Dictionary<TileBase, BaseTileData> _map;

    private void OnEnable()
    {
        BuildMap();
    }

    public void BuildMap()
    {
        _map = new Dictionary<TileBase, BaseTileData>();
        if (allTiles == null) return;
        foreach (var d in allTiles)
        {
            if (d != null && d.Tile != null)
                _map[d.Tile] = d;
        }
    }

    public BaseTileData GetTileData(TileBase tile)
    {
        if (tile == null) return null;
        if (_map == null) BuildMap();
        _map.TryGetValue(tile, out var data);
        return data;
    }

    public BaseTileData GetTileDataByName(string displayOrType)
    {
        if (allTiles == null) return null;
        foreach (var d in allTiles)
        {
            if (d != null && (d.DisplayName == displayOrType || d.Tile.name == displayOrType))
                return d;
        }
        return null;
    }
}
