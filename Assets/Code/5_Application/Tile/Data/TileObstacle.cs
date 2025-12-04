using UnityEngine;

public class TileObstacle : MonoBehaviour
{
    public bool BlocksMovement = true;
    public bool BlocksVision = true;
    public bool IsDestructible = false;

    public ETileBlockType TileBlockType;

    public Vector2Int Size = Vector2Int.one;
    // ถ้า object ใหญ่ 2×2 หรือ 3×2 cell ก็รองรับ
}