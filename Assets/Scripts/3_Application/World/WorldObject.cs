using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldObject :
  MonoBehaviour,
  IDestructible,
  IPoolable<GameObject>
{
  [Header("Placement Flags")]
  [SerializeField] private bool _placementBlocksMovement = true;
  [SerializeField] private bool _placementBlocksVision = true;

  [Header("Obstacle Flags")]
  [SerializeField] private bool _obstacleBlocksMovement = true;
  [SerializeField] private bool _obstacleBlocksVision = true;

  [Header("Grid Settings")]
  [SerializeField] private float _cellSize = 1f;

  [Header("Object Size (in tiles)")]
  [SerializeField] private Vector2Int _objectSize = Vector2Int.one;

  [Header("Placement Footprint")]
  [SerializeField] private Footprint[] _placement;

  [Header("Obstacle Footprints")]
  [SerializeField] private Footprint[] _obstacles;

  [Header("Gizmo")]
  [SerializeField] private bool _drawGizmos = true;

  [Serializable]
  public struct Footprint
  {
    public Vector2Int Size;
    public Vector2Int Offset;
  }

  public bool PlacementBlocksMovement => _placementBlocksMovement;
  public bool PlacementBlocksVision => _placementBlocksVision;
  public bool ObstacleBlocksMovement => _obstacleBlocksMovement;
  public bool ObstacleBlocksVision => _obstacleBlocksVision;

  public event Action<GameObject> OnRequestDestruction;

  public Vector3 GetObjectBottomLeft(float cellSize)
  {
    Vector2 half = new Vector2((_objectSize.x - 1) / 2f, (_objectSize.y - 1) / 2f);
    return transform.position + new Vector3(-half.x * cellSize, -half.y * cellSize, 0);
  }

  public IEnumerable<(Vector3 bottomLeft, Vector2Int size)>
    GetPlacementFootprint(float cellSize)
  {
    Vector3 objBL = GetObjectBottomLeft(cellSize);

    foreach (var fp in _placement)
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
  public void OnSpawnFromPool(GameObject ob) { }

  public void OnReturnToPool(GameObject ob) { }

  private void OnDrawGizmos()
  {
    if (!_drawGizmos) 
      return;

    DrawObjectGizmo();
    DrawFootprints(_placement, PlacementBlocksVision, new Color(0f, 1f, 0f, 0.35f));
    DrawFootprints(_obstacles, ObstacleBlocksVision, new Color(1f, 0f, 0f, 0.35f));
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

  private void DrawFootprints(Footprint[] footprints, bool blocksVision, Color baseColor)
  {
    if (footprints == null)
      return;

    float cellSize = _cellSize;
    Vector3 objBL = GetObjectBottomLeft(cellSize);

    Gizmos.color = blocksVision
      ? baseColor
      : new Color(baseColor.r, baseColor.g, 0f, baseColor.a);

    foreach (var fp in footprints)
    {
      Vector3 world = objBL + new Vector3(
        fp.Offset.x * cellSize,
        fp.Offset.y * cellSize,
        0
      );

      float cellX = Mathf.Floor(world.x / cellSize);
      float cellY = Mathf.Floor(world.y / cellSize);

      Vector3 bottomLeft = new Vector3(
        cellX * cellSize,
        cellY * cellSize,
        0
      );

      Vector3 size = new Vector3(
        fp.Size.x * cellSize,
        fp.Size.y * cellSize,
        0.1f
      );

      Vector3 center = bottomLeft + size * 0.5f;

      Gizmos.DrawCube(center, size);
    }
  }
}