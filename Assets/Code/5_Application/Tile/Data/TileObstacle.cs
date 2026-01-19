using UnityEngine;

public class TileObstacle : MonoBehaviour
{
    [Header("Basic Flags")]
    public bool BlocksMovement = true;
    public bool BlocksVision = true;

    public ETileCapability tileCapability;

    [Header("Object Size (in tiles)")]
    [Tooltip("ขนาดของ object บน grid เช่น 2x2, 3x1 ฯลฯ")]
    public Vector2Int ObjectSize = Vector2Int.one;

    [Header("Obstacle Area (in tiles)")]
    [Tooltip("พื้นที่จริงที่เป็น obstacle เช่น 1x2, 2x1, 2x2")]
    public Vector2Int ObstacleSize = Vector2Int.one;

    [Header("Offset from Object center")]
    [Tooltip("เลื่อน obstacle area เพื่อให้ตรงตำแหน่ง เช่น (0, -0.5f) เพื่อให้ block เฉพาะด้านล่าง")]
    public Vector2 ObstacleOffset = Vector2.zero;

    // Helper: กำหนด bottom-left worldPos ของ object
    public Vector3 GetObjectBottomLeft(float cellSize)
    {
        Vector2 half = new Vector2((ObjectSize.x - 1) / 2f, (ObjectSize.y - 1) / 2f);
        return transform.position + new Vector3(-half.x * cellSize, -half.y * cellSize, 0);
    }

    // Helper: bottom-left ของ obstacle area
    public Vector3 GetObstacleBottomLeft(float cellSize)
    {
        Vector3 objBL = GetObjectBottomLeft(cellSize);
        return objBL + new Vector3(ObstacleOffset.x * cellSize, ObstacleOffset.y * cellSize, 0);
    }
}