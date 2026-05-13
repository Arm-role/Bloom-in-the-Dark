// FlowFieldOwner.cs
using UnityEngine;

public class FlowFieldOwner : MonoBehaviour
{
  [SerializeField] private Vector2Int footprint = new Vector2Int(1, 1);

  // pivotAnchor: ตำแหน่ง pivot ในหน่วย cell (รองรับทศนิยม)
  // เลขคี่  3x3 → (1.0, 1.0) = กลางพอดี
  // เลขคู่  2x2 → (0.5, 0.5) = กลางพอดี (ระหว่าง 4 cell)
  // เลขคู่  4x4 → (1.5, 1.5)
  [SerializeField] private Vector2 pivotAnchor = Vector2.zero;

  public Vector2Int Footprint => footprint;
  public Vector2 PivotAnchor => pivotAnchor;

  // compat — ระบบเก่าที่ยังใช้ PivotOffset (int) ให้ round เอา
  public Vector2Int PivotOffset => new Vector2Int(
      Mathf.RoundToInt(pivotAnchor.x),
      Mathf.RoundToInt(pivotAnchor.y)
  );

  // คืน world positions ของทุก cell ใน footprint ณ ตำแหน่งปัจจุบัน
  public void GetFootprintCells(
      Vector3 worldPos,
      IGridConverter grid,
      System.Collections.Generic.List<Vector3Int> result)
  {
    result.Clear();

    for (int dx = 0; dx < footprint.x; dx++)
    {
      for (int dy = 0; dy < footprint.y; dy++)
      {
        // offset ใน world space จาก pivot
        Vector3 cellWorldPos = worldPos + new Vector3(
            (dx - pivotAnchor.x) * grid.CellSize,
            (dy - pivotAnchor.y) * grid.CellSize,
            0
        );

        result.Add(grid.WorldToCell(cellWorldPos));
      }
    }
  }

#if UNITY_EDITOR
  private static readonly System.Collections.Generic.List<Vector3Int> _gizmoBuffer
      = new System.Collections.Generic.List<Vector3Int>();

  void OnDrawGizmos()
  {
    var manager = FlowFieldManager.Instance;
    if (manager == null) return;

    var grid = manager.world?.GridConverter;
    if (grid == null) return;

    GetFootprintCells(transform.position, grid, _gizmoBuffer);

    foreach (var cell in _gizmoBuffer)
    {
      Vector3 cellWorld = grid.GetCellCenterWorld(cell);

      Gizmos.color = new Color(1f, 0.3f, 1f, 0.2f);
      Gizmos.DrawCube(cellWorld, Vector3.one * grid.CellSize * 0.9f);

      Gizmos.color = new Color(1f, 0.3f, 1f, 0.7f);
      Gizmos.DrawWireCube(cellWorld, Vector3.one * grid.CellSize * 0.9f);
    }

    // จุด pivot จริง (world position ไม่ใช่ cell center)
    Gizmos.color = new Color(1f, 0.9f, 0f, 0.9f);
    Gizmos.DrawWireSphere(transform.position, grid.CellSize * 0.15f);
  }
#endif
}