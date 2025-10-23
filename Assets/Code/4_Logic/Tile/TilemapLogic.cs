using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLogic
{
    private readonly Tilemap _tilemap;

    public TilemapLogic(Tilemap tilemap)
    {
        _tilemap = tilemap;
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return _tilemap.WorldToCell(worldPos);
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPos)
    {
        return _tilemap.GetCellCenterWorld(cellPos);
    }

    public bool CanPlace(Vector3Int cellPos)
    {
        return _tilemap.GetTile(cellPos) == null;
    }

    public void SetTile(Vector3Int cellPos, TileBase tile)
    {
        _tilemap.SetTile(cellPos, tile);
    }

    public void ClearTile(Vector3Int cellPos)
    {
        _tilemap.SetTile(cellPos, null);
    }
}
