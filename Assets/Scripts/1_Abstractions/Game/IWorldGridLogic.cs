using UnityEngine;

public interface IWorldGridLogic
{
  bool[,] GetPlacementValidity(
    Vector2Int origin,
    Vector2Int size,
    System.Func<Vector2Int, Vector3> gridToWorld);
}