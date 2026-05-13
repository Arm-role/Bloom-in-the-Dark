using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemDefinition")]
public class ItemDefinition : ScriptableObject, IItemDefinition
{
  [Header("ItemData")]
  [SerializeField] private ItemKey itemKey;
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