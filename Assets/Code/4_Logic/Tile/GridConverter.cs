using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridConverter
{
    private readonly Tilemap _tilemap;
    private readonly GridLogic _gridLogic;
    public readonly float CellSize;

    public GridConverter(Tilemap tilemap)
    {
        _tilemap = tilemap;
        CellSize = 1;
        _gridLogic = new GridLogic(CellSize, new Vector3(0.5f, 0.5f, 0f));
    }

    public Vector2Int WorldToGrid(Vector3 pointerWorldPos)
    {
        return _gridLogic.WorldToGrid(pointerWorldPos);
    }
    public Vector3 GridToWorld(Vector2Int grid)
    {
        return _gridLogic.GridToWorld(grid);
    }
    public Vector3Int WorldToCell(Vector3 pointerWorldPos)
    {
        return _tilemap.WorldToCell(pointerWorldPos);
    }
    public Vector3 CellToWorld(Vector3Int cell)
    {
        return _tilemap.CellToWorld(cell);
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPos)
    {
        return _tilemap.GetCellCenterWorld(cellPos);
    }
}