using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Placement/Profile")]
public class PlacementProfile : ScriptableObject, IPlacementProfile
{
  [SerializeField] private Vector2Int gridSize;
  [SerializeField] private ObjectKey objectKey;

  public Vector2Int GridSize => gridSize;
  public ObjectKey ObjectKey => objectKey;
}