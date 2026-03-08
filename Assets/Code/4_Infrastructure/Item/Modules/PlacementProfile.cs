using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Placement/Profile")]
public class PlacementProfile : ScriptableObject, IPlacementProfile
{
  [SerializeField] private Vector2Int gridSize;
  [SerializeField] private GameObject prefab;

  public Vector2Int GridSize => gridSize;
  public string PrefabId => prefab.name;
  public GameObject Prefab => prefab;
}
