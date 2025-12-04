using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TilemapService
{
    private readonly Dictionary<ETileLayerType, Tilemap> _tilemaps = new();
    private readonly GridConverter _gridConverter;
    private readonly TileLibrary _tileLibrary;
    private readonly WorldTileManager _worldTileManager;

    public TilemapService(
      List<TilemapLayer> tilemaps,
      GridConverter gridConverter,
      TileLibrary tileLibrary,
      WorldTileManager worldTileManager)
    {
        foreach (var tl in tilemaps)
        {
            _tilemaps[tl.layerType] = tl.tilemap;
        }

        _gridConverter = gridConverter;
        _tileLibrary = tileLibrary;
        _worldTileManager = worldTileManager;
    }

    public bool PlaceTile(Vector3 worldPos, string tileName, ETileLayerType layer)
    {
        if (!_tilemaps.TryGetValue(layer, out var tilemap))
            return false;

        var tileBaseData = _tileLibrary.GetTileBaseDataByName(tileName);
        if (tileBaseData == null) return false;

        Vector3Int cell = _gridConverter.WorldToCell(worldPos);
        var state = _worldTileManager.GetOrCreateTileState(cell);

        if (state.HasPlacedObject || state.GetTile(layer) != null)
            return false;

        tilemap.SetTile(cell, tileBaseData.Tiles[0]);
        state.SetTile(layer, tileBaseData);

        _worldTileManager.UpdateTileInteractable(state);

        return true;
    }

    public bool RemoveTile(Vector3 worldPos, ETileLayerType layer)
    {
        if (!_tilemaps.TryGetValue(layer, out var tilemap))
            return false;

        Vector3Int cell = _gridConverter.WorldToCell(worldPos);
        var state = _worldTileManager.GetTileState(cell);
        if (state == null) return false;

        tilemap.SetTile(cell, null);
        state.SetTile(layer, null);

        if (!state.HasPlacedObject && state.tiles.Count == 0)
            _worldTileManager.RemoveTileState(cell);

        _worldTileManager.UpdateTileInteractable(state);

        return true;
    }

    public TileBaseData GetTileDataAt(Vector3 worldPos, ETileLayerType layer)
    {
        if (!_tilemaps.TryGetValue(layer, out var tilemap))
            return null;

        Vector3Int cell = _gridConverter.WorldToCell(worldPos);
        var state = _worldTileManager.GetTileState(cell);
        return state?.GetTile(layer);
    }
}