using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapService
{
    private readonly Tilemap _tilemap;
    private readonly GridConverter _gridConverter;
    private readonly WorldTileManager _worldTileManager;
    private readonly ETileLayerType _layerType;

    public TilemapService(
        Tilemap tilemap,
        GridConverter gridConverter,
        WorldTileManager worldTileManager,
        ETileLayerType layerType)
    {
        _tilemap = tilemap;
        _gridConverter = gridConverter;
        _worldTileManager = worldTileManager;
        _layerType = layerType;
    }

    public bool PlaceTile(Vector3 worldPos, TileBaseData tileData)
    {
        if (tileData == null || tileData.Tile == null)
            return false;

        Vector3Int cellPos = _gridConverter.WorldToCell(worldPos);
        var state = _worldTileManager.GetOrCreateTileState(cellPos);

        // ถ้ามีของวางอยู่แล้ว (object หรือ tile ใน layer เดียวกัน)
        if (state.IsOccupied || state.GetTile(_layerType) != null)
            return false;

        _tilemap.SetTile(cellPos, tileData.Tile);
        state.SetTile(_layerType, tileData);

        return true;
    }

    public bool RemoveTile(Vector3 worldPos)
    {
        Vector3Int cellPos = _gridConverter.WorldToCell(worldPos);
        var state = _worldTileManager.GetTileState(cellPos);
        if (state == null) return false;

        var tile = _tilemap.GetTile(cellPos);
        if (tile == null) return false;

        _tilemap.SetTile(cellPos, null);
        state.SetTile(_layerType, null);

        // ถ้าไม่มี tile ชั้นใดเหลือเลย และไม่มี object → ลบ state ทิ้ง
        if (!state.IsOccupied && state.tiles.Count == 0)
            _worldTileManager.RemoveTileState(cellPos);

        return true;
    }

    public TileBaseData GetTileDataAtWorld(Vector3 worldPos)
    {
        Vector3Int cellPos = _gridConverter.WorldToCell(worldPos);

        var state = _worldTileManager.GetTileState(cellPos);
        if (state == null) return null;

        var tilebaseData = state.GetTile(_layerType);
        if (tilebaseData == null) return null;

        return tilebaseData;
    }

    public Tilemap GetTilemap() => _tilemap;
    public ETileLayerType GetLayerType() => _layerType;
}
