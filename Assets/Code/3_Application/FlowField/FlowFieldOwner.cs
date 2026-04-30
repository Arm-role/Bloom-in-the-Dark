using UnityEngine;

public class FlowFieldOwner : MonoBehaviour
{
  [SerializeField] private Vector2Int footprint = new Vector2Int(1, 1);

  // offset ของ pivot บน footprint
  // (0,0) = bottom-left corner, (1,1) สำหรับ 2x2 = cell ที่สอง
  // ถ้า pivot อยู่กลาง fp=3 → set (1,1)
  [SerializeField] private Vector2Int pivotOffset = Vector2Int.zero;

  public Vector2Int Footprint => footprint;
  public Vector2Int PivotOffset => pivotOffset;
  public int Radius => Mathf.Max(footprint.x, footprint.y) / 2;

#if UNITY_EDITOR
  void OnDrawGizmos()
  {
    var manager = FlowFieldManager.Instance;
    if (manager == null) return;

    var grid = manager.world?.GridConverter;
    if (grid == null) return;

    // ✅ snap transform.position ให้ตรง cell ก่อน
    Vector3Int pivotCell = grid.WorldToCell(transform.position);
    Vector3 snappedPivot = grid.GetCellCenterWorld(pivotCell);

    for (int dx = 0; dx < footprint.x; dx++)
    {
      for (int dy = 0; dy < footprint.y; dy++)
      {
        var cell = new Vector3Int(
            pivotCell.x + (dx - pivotOffset.x),
            pivotCell.y + (dy - pivotOffset.y),
            0);

        Vector3 cellWorld = grid.GetCellCenterWorld(cell); // ✅ ใช้ center เสมอ

        Gizmos.color = new Color(1f, 0.3f, 1f, 0.2f);
        Gizmos.DrawCube(cellWorld, Vector3.one * grid.CellSize * 0.9f);

        Gizmos.color = new Color(1f, 0.3f, 1f, 0.7f);
        Gizmos.DrawWireCube(cellWorld, Vector3.one * grid.CellSize * 0.9f);
      }
    }

    // pivot cell highlight
    Gizmos.color = new Color(1f, 0.9f, 0f, 0.9f);
    Gizmos.DrawWireCube(
        grid.GetCellCenterWorld(pivotCell), // ✅ center ไม่ใช่ corner
        Vector3.one * grid.CellSize * 0.95f);
  }
#endif
}