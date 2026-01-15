using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRenderer
{
    private readonly Dictionary<ETileLayerType, Tilemap> _tilemaps = new();

    public TilemapRenderer(
        List<TilemapLayer> layers)
    {
        foreach (var layer in layers)
        {
            _tilemaps[layer.layerType] = layer.tilemap;
        }
    }

    public void SetTile(
        Vector3Int cellPos,
        ETileLayerType layer,
        TileBase tile)
    {
        _tilemaps[layer].SetTile(cellPos, tile);
    }

    public void ClearTile(
        Vector3Int cellPos,
        ETileLayerType layer)
    {
        _tilemaps[layer].SetTile(cellPos, null);
    }
}