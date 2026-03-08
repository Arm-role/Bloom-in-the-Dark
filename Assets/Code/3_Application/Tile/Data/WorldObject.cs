using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject :
  MonoBehaviour,
  IDestructible,
  IPoolable<GameObject>
{
  [Header("Basic Flags")] [SerializeField]
  private bool _blocksMovement = true;

  [SerializeField] private bool _blocksVision = true;

  [Header("Grid Settings")] [SerializeField]
  private float _cellSize = 1f;

  [Header("Object Size (in tiles)")] [SerializeField]
  private Vector2Int _objectSize = Vector2Int.one;

  [Header("Obstacle Footprints")] [SerializeField]
  private ObstacleFootprint[] _obstacles;

  [Header("Gizmo")] [SerializeField] private bool _drawGizmos = true;

  [Serializable]
  public struct ObstacleFootprint
  {
    public Vector2Int Size; // ขนาดเป็น tile
    public Vector2 Offset; // offset จาก object origin (เป็น tile)
  }

  public bool BlocksMovement => _blocksMovement;
  public bool BlocksVision => _blocksVision;

  public event Action<GameObject> OnRequestDestruction;
  
  public Vector3 GetObjectBottomLeft(float cellSize)
  {
    Vector2 half = new Vector2((_objectSize.x - 1) / 2f, (_objectSize.y - 1) / 2f);
    return transform.position + new Vector3(-half.x * cellSize, -half.y * cellSize, 0);
  }

  public IEnumerable<(Vector3 bottomLeft, Vector2Int size)>
    GetObstacleFootprints(float cellSize)
  {
    Vector3 objBL = GetObjectBottomLeft(cellSize);

    foreach (var fp in _obstacles)
    {
      Vector3 world = objBL + new Vector3(
        fp.Offset.x * cellSize,
        fp.Offset.y * cellSize,
        0
      );

      // snap to grid
      float cellX = Mathf.Floor(world.x / cellSize);
      float cellY = Mathf.Floor(world.y / cellSize);

      Vector3 snappedBL = new Vector3(
        cellX * cellSize,
        cellY * cellSize,
        world.z
      );

      yield return (snappedBL, fp.Size);
    }
  }
  
  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  public bool IsAlive { get; set; }
  public void OnSpawnFromPool(GameObject ob)
  {
        
  }

  public void OnReturnToPool(GameObject ob)
  {
        
  }

  private void OnDrawGizmos()
  {
    if (_drawGizmos)
    {
      DrawObjectGizmo();
      DrawObstacleGizmo();
    }
  }

  private void DrawObjectGizmo()
  {
    Gizmos.color = new Color(0f, 0.6f, 1f, 0.4f); // ฟ้า

    float cellSize = _cellSize;

    Vector3 bottomLeft = GetSnappedObjectBottomLeft(cellSize);

    Vector3 size = new Vector3(
      _objectSize.x * cellSize,
      _objectSize.y * cellSize,
      0.1f
    );

    Vector3 center = bottomLeft + new Vector3(
      size.x * 0.5f,
      size.y * 0.5f,
      0f
    );

    Gizmos.DrawWireCube(center, size);
  }

  private Vector3 GetSnappedObjectBottomLeft(float cellSize)
  {
    Vector3 bl = GetObjectBottomLeft(cellSize);

    float cellX = Mathf.Floor(bl.x / cellSize);
    float cellY = Mathf.Floor(bl.y / cellSize);

    return new Vector3(
      cellX * cellSize,
      cellY * cellSize,
      bl.z
    );
  }

  private void DrawObstacleGizmo()
  {
    if (_obstacles == null)
      return;

    foreach (var fp in _obstacles)
    {
      Gizmos.color = BlocksVision
        ? new Color(1f, 0f, 0f, 0.5f)
        : new Color(1f, 0.6f, 0f, 0.5f);

      Vector3 objBL = GetObjectBottomLeft(_cellSize);
      Vector3 world = objBL + new Vector3(
        fp.Offset.x * _cellSize,
        fp.Offset.y * _cellSize,
        0
      );

      float cellX = Mathf.Floor(world.x / _cellSize);
      float cellY = Mathf.Floor(world.y / _cellSize);

      Vector3 bottomLeft = new Vector3(
        cellX * _cellSize,
        cellY * _cellSize,
        0
      );

      Vector3 size = new Vector3(
        fp.Size.x * _cellSize,
        fp.Size.y * _cellSize,
        0.1f
      );

      Vector3 center = bottomLeft + size * 0.5f;

      Gizmos.DrawCube(center, size);
    }
  }
}