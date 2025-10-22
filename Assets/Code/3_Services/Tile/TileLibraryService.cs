using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileLibraryService
{
    private readonly Dictionary<string, TileBase> _tileMapFromID = new Dictionary<string, TileBase>();

    public TileLibraryService(List<TileData> allTileData)
    {
        foreach (var data in allTileData)
        {
            if (data != null && !_tileMapFromID.ContainsKey(data.ID))
            {
                _tileMapFromID.Add(data.ID, data.DisplayTile);
            }
        }
    }

    public TileBase GetTileByID(string id)
    {
        _tileMapFromID.TryGetValue(id, out var tile);
        return tile;
    }
}