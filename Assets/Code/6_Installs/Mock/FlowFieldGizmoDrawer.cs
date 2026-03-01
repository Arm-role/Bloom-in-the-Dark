using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlowFieldGizmoDrawer : MonoBehaviour
{
    public FlowFieldManager manager;
    public WorldTileManager tileManager;
    public string FieldKey = "AttackPlayer";

    [Header("Visualization")]
    [Range(1, 6)] public int Density = 1;
    [Range(0.1f, 1f)] public float ArrowScale = 0.55f;
    public float ArrowThickness = 2f;
    public bool DrawBlocked = true;
    public bool DrawTargets = true;

#if UNITY_EDITOR
    // ===== Cache เพื่อไม่ให้ตำแหน่ง tile แกว่ง =====
    private struct TileInfo
    {
        public Vector3Int cell;
        public Vector3 pos;
        public bool blocked;
    }

    private List<TileInfo> cachedTiles = new List<TileInfo>();
    private FlowField cachedField;
    private float cellSize;

    public void Start()
    {
        TileDomainEvents.OnObstacleScanCompleted += Setitng;
    }

    private void Setitng()
    {
        if (tileManager == null) return;

        cellSize = tileManager.GridConverter.CellSize;

        // Cache tile positions 1 ครั้ง → ไม่ jitter
        cachedTiles.Clear();
        foreach (var t in tileManager.GetAllCells())
        {
            Vector3Int c = t.CellPos;

            TileInfo info = new TileInfo()
            {
                cell = c,
                pos = t.WorldCenter,
                blocked = t.BlocksMovement
            };

            cachedTiles.Add(info);
        }
    }

    private void Update()
    {
        if (manager == null) return;

        FlowField f = manager.GetField(FieldKey);
        if (f != cachedField)
            cachedField = f;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (manager == null || cachedField == null) return;

        DrawFlowArrows();
        if (DrawTargets) DrawTargetTiles();
    }

    // ==================================================
    private void DrawFlowArrows()
    {
        Handles.color = Color.green;

        foreach (var tile in cachedTiles)
        {
            if ((tile.cell.x + tile.cell.y) % Density != 0)
                continue;

            Vector2 dir = manager.GetDirection(FieldKey, tile.cell);

            if (dir.sqrMagnitude < 0.01f)
            {
                if (DrawBlocked && tile.blocked)
                    DrawBlockedCell(tile.pos);
                continue;
            }

            DrawArrow(tile.pos, dir.normalized);
        }
    }

    private void DrawArrow(Vector3 pos, Vector2 dir)
    {
        float cs = cellSize;

        Handles.color = new Color(0, 1f, 0.2f, 1f);

        float lineLength = cs * ArrowScale;
        float headLength = lineLength * 0.35f;
        float headAngle = 25f; // คมขึ้น

        Vector3 end = pos + (Vector3)(dir * lineLength);

        // ลำเส้น
        Handles.DrawAAPolyLine(ArrowThickness * 1.6f, pos, end);

        // หัวลูกศร
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * (-dir) * headLength;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * (-dir) * headLength;

        Handles.DrawAAPolyLine(ArrowThickness * 1.6f, end, end + right);
        Handles.DrawAAPolyLine(ArrowThickness * 1.6f, end, end + left);
    }

    private void DrawBlockedCell(Vector3 pos)
    {
        float cs = cellSize;

        Handles.color = new Color(1, 0, 0, 0.5f);
        Handles.DrawSolidRectangleWithOutline(
            new Vector3[]
            {
                pos + new Vector3(-cs/2, -cs/2),
                pos + new Vector3( cs/2, -cs/2),
                pos + new Vector3( cs/2,  cs/2),
                pos + new Vector3(-cs/2,  cs/2)
            },
            new Color(1, 0, 0, 0.14f),
            Color.red
        );
    }

    // ==================================================
    private void DrawTargetTiles()
    {
        float cs = cellSize * 0.35f;

        foreach (var tile in cachedTiles)
        {
            int lx = tile.cell.x - cachedField.originCell.x;
            int ly = tile.cell.y - cachedField.originCell.y;

            if (!cachedField.IsInside(new Vector2Int(lx, ly))) continue;

            if (cachedField.integrationField[lx, ly] == 0)
            {
                Handles.color = Color.yellow;
                Handles.DrawSolidDisc(tile.pos, Vector3.forward, cs);
            }
        }
    }
#endif
}
