using UnityEngine;

public class FlowFieldDebugDrawer : MonoBehaviour
{
  public FlowFieldChannelKey key;

  [Header("Agent Footprint")]
  public Vector2Int footprint = new Vector2Int(1, 1);

  [Header("Debug Options")]
  public bool drawGizmos = true;
  public bool drawFlow = true;
  public bool drawIntegration = false;
  public bool drawCost = false;
  public bool drawUnreachable = true;
  public bool drawFootprintOverlay = false;

  public float arrowScale = 0.4f;

  private IGridConverter Grid =>
      FlowFieldManager.Instance?.world?.GridConverter;

  void OnDrawGizmos()
  {
    if (!drawGizmos) return;
    if (!Application.isPlaying) return;

    var manager = FlowFieldManager.Instance;
    if (manager == null) return;

    var grid = Grid;
    if (grid == null) return;

    var field = manager.GetField(key, footprint);
    if (field == null) return;

    for (int x = 0; x < field.width; x++)
      for (int y = 0; y < field.height; y++)
      {
        var idx = new Vector2Int(x, y);

        // center world ของ footprint block นี้ทั้งก้อน
        Vector3 blockCenter = GetFootprintBlockCenter(field, x, y, grid);

        if (drawFlow) DrawFlow(field, idx, blockCenter, grid);
        if (drawCost) DrawCost(field, idx, blockCenter, grid);
        if (drawIntegration) DrawIntegration(field, idx, blockCenter);
        if (drawFootprintOverlay) DrawFootprintOverlay(blockCenter, grid);
      }
  }

  // คำนวณ world center ของ footprint block ที่มี bottom-left อยู่ที่ cell (x,y)
  Vector3 GetFootprintBlockCenter(FlowField field, int x, int y, IGridConverter grid)
  {
    // bottom-left cell ของ block
    Vector3Int originCell = new Vector3Int(
        field.originCell.x + x,
        field.originCell.y + y,
        0);

    Vector3 bottomLeft = grid.GetCellCenterWorld(originCell);

    // เลื่อนไปหา center ของ block
    float offsetX = (footprint.x - 1) * 0.5f * grid.CellSize;
    float offsetY = (footprint.y - 1) * 0.5f * grid.CellSize;

    return bottomLeft + new Vector3(offsetX, offsetY, 0f);
  }

  void DrawFlow(FlowField field, Vector2Int idx, Vector3 blockCenter, IGridConverter grid)
  {
    Vector2 dir = field.GetDirection(idx);

    if (dir == Vector2.zero)
    {
      if (drawUnreachable)
      {
        Gizmos.color = new Color(1f, 0.4f, 0.4f, 0.25f);
        // ครอบทั้ง footprint block
        Gizmos.DrawCube(blockCenter, new Vector3(
            footprint.x * grid.CellSize * 0.9f,
            footprint.y * grid.CellSize * 0.9f,
            0.1f));
      }
      return;
    }

    Gizmos.color = Color.green;

    float arrowLen = grid.CellSize * Mathf.Min(footprint.x, footprint.y) * arrowScale;
    Vector3 to = blockCenter + (Vector3)dir.normalized * arrowLen;

    Gizmos.DrawLine(blockCenter, to);
    Gizmos.DrawSphere(to, grid.CellSize * 0.08f);
  }

  void DrawCost(FlowField field, Vector2Int idx, Vector3 blockCenter, IGridConverter grid)
  {
    if (field.GetCost(idx) != FlowField.COST_IMPASSABLE) return;

    Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
    Gizmos.DrawCube(blockCenter, new Vector3(
        footprint.x * grid.CellSize * 0.9f,
        footprint.y * grid.CellSize * 0.9f,
        0.1f));
  }

  void DrawFootprintOverlay(Vector3 blockCenter, IGridConverter grid)
  {
    Gizmos.color = new Color(0f, 0.6f, 1f, 0.15f);
    Gizmos.DrawCube(blockCenter, new Vector3(
        footprint.x * grid.CellSize,
        footprint.y * grid.CellSize,
        1f));
  }

  void DrawIntegration(FlowField field, Vector2Int idx, Vector3 blockCenter)
  {
#if UNITY_EDITOR
    int v = field.GetIntegration(idx);
    if (v == FlowField.INTEGRATION_MAX) return;
    UnityEditor.Handles.Label(blockCenter, v.ToString());
#endif
  }
}