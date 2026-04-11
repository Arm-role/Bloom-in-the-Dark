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

  public float arrowScale = 0.4f;


  void OnDrawGizmos()
  {
    if (!drawGizmos)
      return;

    if (!Application.isPlaying)
      return;

    if (FlowFieldManager.Instance == null)
      return;


    var field =
        FlowFieldManager.Instance.GetField(
            key,
            footprint
        );

    if (field == null)
      return;


    var grid =
        FlowFieldManager.Instance
        .world
        .GridConverter;


    for (int x = 0; x < field.width; x++)
    {
      for (int y = 0; y < field.height; y++)
      {
        Vector3 world =
            grid.CellToWorld(
                new Vector3Int(
                    field.originCell.x + x,
                    field.originCell.y + y,
                    0
                )
            );


        if (drawFlow)
          DrawFlow(field, x, y, world);

        if (drawCost)
          DrawCost(field, x, y, world);

        if (drawIntegration)
          DrawIntegration(field, x, y, world);
      }
    }
  }


  void DrawFlow(
      FlowField field,
      int x,
      int y,
      Vector3 world
  )
  {
    Vector2 dir =
        field.GetDirection(
            new Vector2Int(x, y)
        );

    if (dir == Vector2.zero)
      return;


    Gizmos.color = Color.green;

    Vector3 to =
        world +
        (Vector3)dir.normalized *
        arrowScale;

    Gizmos.DrawLine(world, to);

    Gizmos.DrawSphere(
        to,
        0.05f
    );
  }


  void DrawCost(
      FlowField field,
      int x,
      int y,
      Vector3 world
  )
  {
    byte cost =
        field.GetCost(
            new Vector2Int(x, y)
        );

    if (cost ==
        FlowField.COST_IMPASSABLE)
    {
      Gizmos.color =
          new Color(
              1,
              0,
              0,
              0.3f
          );

      Gizmos.DrawCube(
          world,
          Vector3.one * 0.4f
      );
    }
  }


  void DrawIntegration(
      FlowField field,
      int x,
      int y,
      Vector3 world
  )
  {
#if UNITY_EDITOR

    UnityEditor.Handles.Label(
        world,
        field
        .GetIntegration(
            new Vector2Int(x, y)
        )
        .ToString()
    );

#endif
  }
}
