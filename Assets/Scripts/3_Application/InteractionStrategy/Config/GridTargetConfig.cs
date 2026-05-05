using UnityEngine;

public sealed class GridTargetConfig : ITargetingConfig
{
  public Vector2Int Size { get; }
  public float MaxRange { get; }

  public GridTargetConfig(Vector2Int size, float maxRange)
  {
    Size = size;
    MaxRange = maxRange;
  }
}