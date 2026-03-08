using System;
using UnityEngine;

public class FlowField
{
    public const byte COST_IMPASSABLE = 255;
    public const byte COST_STRAIGHT = 10;
    public const byte COST_DIAGONAL = 14;

    public int width;
    public int height;
    public Vector3 originWorld;
    public Vector3Int originCell;
    public float cellSize;

    public byte[,] costField;
    public int[,] integrationField;
    public Vector2[,] flowVectorField;

    public FlowField(int width, int height, Vector3 originWorld, float cellSize, Vector3Int originCell)
    {
        this.width = width;
        this.height = height;
        this.originWorld = originWorld;
        this.cellSize = cellSize;
        this.originCell = originCell;

        costField = new byte[width, height];
        integrationField = new int[width, height];
        flowVectorField = new Vector2[width, height];
    }

    public bool IsInside(Vector2Int idx)
    {
        return idx.x >= 0 && idx.x < width && idx.y >= 0 && idx.y < height;
    }

    public void SetCost(Vector2Int idx, byte c) => costField[idx.x, idx.y] = c;
    public byte GetCost(Vector2Int idx) => costField[idx.x, idx.y];

    public void SetIntegration(Vector2Int idx, int v) => integrationField[idx.x, idx.y] = v;
    public int GetIntegration(Vector2Int idx) => integrationField[idx.x, idx.y];

    public void SetDirection(Vector2Int idx, Vector2 v) => flowVectorField[idx.x, idx.y] = v;
    public Vector2 GetDirection(Vector2Int idx) => flowVectorField[idx.x, idx.y];

    // helper to compute world center of an index
    public Vector3 IndexToWorld(int ix, int iy, IGridConverter grid)
    {
        Vector3Int cell = new Vector3Int(originCell.x + ix, originCell.y + iy, 0);
        return grid.CellToWorld(cell);
    }
}
