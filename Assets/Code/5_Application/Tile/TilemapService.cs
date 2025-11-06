using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TilemapService
{
    private readonly Tilemap _tilemap;
    private readonly GridConverter _gridLogic;
    private readonly WorldTileState _tileState;
    public TilemapService(Tilemap tilemap, GridConverter gridlogic, WorldTileState tileState)
    {
        _tilemap = tilemap;
        _gridLogic = gridlogic;
        _tileState = tileState;
    }

    public bool PlaceTile(Vector3 worldPos, BaseTileData tileData)
    {
        if (tileData == null) return false;

        Vector3Int cellPos = _gridLogic.WorldToCell(worldPos);

        _tilemap.SetTile(cellPos, tileData.Tile);
        _tileState.SetTileData(cellPos, tileData);

        return true;
    }

    public void RemoveTile(Vector3 worldPos)
    {
        Vector3Int cellPos = _gridLogic.WorldToCell(worldPos);
        TileBase tile = _tilemap.GetTile(cellPos);

        if (tile != null)
        {
            _tilemap.SetTile(cellPos, null);
            _tileState.ClearTileData(cellPos);
        }
    }

    public BaseTileData GetTileDataAtWorld(Vector3 worldPos)
    {
        Vector3Int cellPos = _gridLogic.WorldToCell(worldPos);
        _tileState.TryGetTileData(cellPos, out var data);
        return data;
    }

    public Tilemap GetTilemap() => _tilemap;
}
