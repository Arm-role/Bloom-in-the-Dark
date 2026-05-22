#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class TileLayerRenderer
{
    private readonly Dictionary<ETileLayerType, Tilemap> _tilemaps = new();

    public TileLayerRenderer(List<TilemapLayer> layers)
    {
        foreach (var layer in layers)
        {
            _tilemaps[layer.layerType] = layer.tilemap;
        }
    }

    public void SetTile(
        Vector3Int cellPos,
        ETileLayerType layer,
        TileBase? tile)
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
