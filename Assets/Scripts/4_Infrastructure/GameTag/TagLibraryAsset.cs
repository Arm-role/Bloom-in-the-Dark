using UnityEngine;

[CreateAssetMenu(menuName = "GameTags/Library")]
public class TagLibraryAsset : ScriptableObject, ITagLibraryAsset
{
  [Header("Capability")]
  [SerializeField] private GameTagAsset[] tags;
  public GameTagAsset[] Tags => tags;
}
