using UnityEngine;

public interface IPlacementProfile
{
  public Vector2Int GridSize { get; }
  public ObjectKey ObjectKey { get; }
}