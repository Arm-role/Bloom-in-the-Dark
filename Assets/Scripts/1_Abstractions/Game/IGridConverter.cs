using UnityEngine;

public interface IGridConverter
{
  float CellSize { get; }

  Vector2Int WorldToGrid(Vector3 pointerWorldPos);
  Vector3 GridToWorld(Vector2Int grid);
  Vector3Int WorldToCell(Vector3 pointerWorldPos);
  Vector3 CellToWorld(Vector3Int cell);
  Vector3 GetCellCenterWorld(Vector3Int cellPos);
}