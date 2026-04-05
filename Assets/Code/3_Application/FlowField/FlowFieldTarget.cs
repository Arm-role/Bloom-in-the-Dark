using System.Collections.Generic;
using UnityEngine;

public class FlowFieldTarget : MonoBehaviour
{
  [SerializeField] private FlowFieldChannelKey flowKey;
  [SerializeField] private float baseThreat;
  [SerializeField] private bool isObjectiveTarget;
  [SerializeField] private Vector2Int size;
  public FlowFieldChannelKey FlowKey => flowKey;
  public float BaseThreat => baseThreat;
  public bool IsObjectiveTarget => isObjectiveTarget;
  
  public IEnumerable<Vector3> GetTargetCells()
  {
    var grid = FlowFieldManager.Instance.world.GridConverter;

    Vector3Int center = grid.WorldToCell(transform.position);

    int halfX = size.x / 2;
    int halfY = size.y / 2;

    for (int x = -halfX; x <= halfX; x++)
    {
      for (int y = -halfY; y <= halfY; y++)
      {
        yield return
            grid.CellToWorld(
                new Vector3Int(
                    center.x + x,
                    center.y + y,
                    0
                )
            );
      }
    }
  }
}