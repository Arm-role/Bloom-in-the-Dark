using UnityEngine;

public class CircleZone : IWorldZone
{
  public Vector3 Center { get; set; }
  public float Radius { get; set; }
  public CellZoneFlags Flags { get; set; }

  public void SetFlags(CellZoneFlags flags)
  {
    Flags = flags;
  }

  public CircleZone(Vector3 center, float radius, CellZoneFlags flags)
  {
    Center = center;
    Radius = radius;
    Flags = flags;
  }

  public bool Contains(Vector3 worldPos)
  {
    return (worldPos - Center).sqrMagnitude <= Radius * Radius;
  }
}