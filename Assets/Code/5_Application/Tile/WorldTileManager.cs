using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WorldTileManager : MonoBehaviour
{
    private List<TilemapLayer> _tilemapLayers = new();
    private TileLibrary _tileLibrary;

    private Dictionary<Vector3Int, TileBaseDataState> _worldTiles = new();
    public void Initialize(List<TilemapLayer> tilemapLayers, TileLibrary tileLibrary)
    {
        _tilemapLayers = tilemapLayers;
        _tileLibrary = tileLibrary;

        _worldTiles.Clear();
        foreach (var layer in tilemapLayers)
        {
            if (layer.tilemap == null) continue;
            ScanLayer(layer.layerType, layer.tilemap);
        }

        Debug.Log($"✅ WorldTileManager initialized with {_worldTiles.Count} tiles");
    }

    private void ScanLayer(ETileLayerType layerType, Tilemap map)
    {
        var bounds = map.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = map.GetTile(cellPos);
                if (tile == null) continue;

                if (!_worldTiles.TryGetValue(cellPos, out var state))
                {
                    state = new TileBaseDataState(cellPos);
                    _worldTiles.Add(cellPos, state);
                }

                var tileData = _tileLibrary.GetTileData(tile);
                state.SetTile(layerType, tileData);
            }
        }
    }

    public TileBaseDataState GetTileState(Vector3Int cellPos)
    {
        _worldTiles.TryGetValue(cellPos, out var state);
        return state;
    }

    public bool TryGetTilemap(ETileLayerType layerType, out Tilemap tilemap)
    {
        tilemap = null;

        foreach (var tilemapLayer in _tilemapLayers)
        {
            if (tilemapLayer.layerType == layerType)
            {
                tilemap = tilemapLayer.tilemap;
                return true;
            }
        }

        return false;
    }

    public bool TryPlaceObject(Vector3Int cellPos, GameObject obj)
    {
        if (!_worldTiles.TryGetValue(cellPos, out var state))
            return false;

        if (state.IsOccupied) return false;

        state.placedObject = obj;
        return true;
    }

    public void RemoveObject(Vector3Int cellPos)
    {
        if (_worldTiles.TryGetValue(cellPos, out var state))
            state.placedObject = null;
    }

    public bool IsOccupied(Vector3Int cellPos)
    {
        return _worldTiles.TryGetValue(cellPos, out var state) && state.IsOccupied;
    }

    public TileBaseDataState GetOrCreateTileState(Vector3Int pos)
    {
        if (!_worldTiles.TryGetValue(pos, out var state))
        {
            state = new TileBaseDataState(pos);
            _worldTiles.Add(pos, state);
        }
        return state;
    }

    public void RemoveTileState(Vector3Int pos)
    {
        _worldTiles.Remove(pos);
    }

}