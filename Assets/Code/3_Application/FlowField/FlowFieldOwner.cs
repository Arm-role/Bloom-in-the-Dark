using UnityEngine;

public class FlowFieldOwner : MonoBehaviour
{
  [SerializeField]
  private Vector2Int footprint = new Vector2Int(1, 1);

  public Vector2Int Footprint => footprint;

  public int Radius =>
      Mathf.Max(footprint.x, footprint.y) / 2;
}