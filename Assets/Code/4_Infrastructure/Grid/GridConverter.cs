using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridConverter : IGridConverter
{
    private readonly Tilemap _tilemap;
    private readonly GridLogic _gridLogic;
    private readonly float _cellSize;

    public float CellSize => _cellSize;
    
    public GridConverter(Tilemap tilemap)
    {
        _tilemap = tilemap;
        _cellSize = 1;
        _gridLogic = new GridLogic(_cellSize, new Vector3(0.5f, 0.5f, 0f));
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