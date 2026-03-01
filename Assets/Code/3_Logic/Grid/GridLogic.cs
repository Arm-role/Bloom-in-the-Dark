using System;
using System.Collections.Generic;
using UnityEngine;

public class GridLogic
{
    private readonly float _cellSize;
    private readonly Vector3 _gridOrigin;

    public GridLogic(float cellSize, Vector3 gridOrigin)
    {
        _cellSize = cellSize;
        _gridOrigin = gridOrigin;
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 relativePos = worldPosition - _gridOrigin;
        int x = Mathf.RoundToInt((relativePos.x / _cellSize));
        int y = Mathf.RoundToInt((relativePos.y / _cellSize));
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2 gridPosition)
    {
        float x = gridPosition.x * _cellSize;
        float y = gridPosition.y * _cellSize;
        return new Vector3(x, y, 0) + _gridOrigin;
    }
}