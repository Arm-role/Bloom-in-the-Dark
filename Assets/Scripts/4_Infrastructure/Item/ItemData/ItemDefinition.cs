using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemDefinition")]
public class ItemDefinition : ScriptableObject, IItemDefinition
{
  [Header("ItemData")]
  [SerializeField] private ItemKey itemKey;
  [Tooltip("ชื่อที่แสดงให้ผู้เล่นเห็น — เว้นว่างจะ fallback ใช้ชื่อ ItemKey")]
  [SerializeField] private string displayName;
  [SerializeField] private GameTagAsset[] tags;
  [SerializeField] private EItemRole itemRole;
  [SerializeField] private int maxStackSize;

  [Header("Modules")]
  [SerializeField] private SkillDefinition skillDefinition;
  [SerializeField] private PlacementProfile placementProfile;
  [SerializeField] private ItemInteractionProfile interactionProfile;
  [SerializeField] private ItemInteractionCapability interactionCapability;

  private int _id;
  public int ID
  {
    get
    {
      if (_id != itemKey.RuntimeTag.Hash)
      {
        _id = itemKey.RuntimeTag.Hash;
      }

      return _id;
    }
  }
  public string Name => itemKey.name;

  // ชื่อแสดงผล — ผู้เล่นเขียนเองได้ ; ว่าง = fallback ชื่อ ItemKey (Name ยังเป็น identifier เดิม)
  public string DisplayName =>
    string.IsNullOrWhiteSpace(displayName) ? itemKey.name : displayName;

  public GameTag Key => itemKey.RuntimeTag;

  public GameTagContainer CreateTagContainer()
  {
    var container = new GameTagContainer();

    foreach (var tag in tags)
      container.Add(tag.RuntimeTag);

    return container;
  }

  public EItemRole Role => itemRole;
  public int MaxStackSize => maxStackSize;

  public ISkillDefinition Skill => skillDefinition;
  public IPlacementProfile PlacementProfile => placementProfile;
  public IItemInteractionProfile InteractionProfile => interactionProfile;
  public IItemInteractionCapability InteractionCapability => interactionCapability;


  private GameTagContainer _tags;

  public GameTagContainer Tags
  {
    get
    {
      if (_tags == null)
      {
        _tags = new GameTagContainer();

        foreach (var tag in tags)
          _tags.Add(tag.RuntimeTag);
      }

      return _tags;
    }
  }

  public bool HasTag(GameTag tag)
  {
    return Tags.Has(tag);
  }
}