using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject, IItemData
{
  [Header("ItemData")] 
  [SerializeField] private int itemId;
  [SerializeField] private string itemName;
  [SerializeField] private Sprite icon;

  [Header("Modules")] 
  [SerializeField] private SkillDefinition skillDefinition;
  [SerializeField] private PlacementProfile placementProfile;
  [SerializeField] private ItemInteractionProfile interactionProfile;
  [SerializeField] private ItemInteractionCapability interactionCapability;

  public int ID => itemId;
  public string Name => itemName;
  public Sprite Icon => icon;

  public SkillDefinition Skill => skillDefinition;
  public PlacementProfile PlacementProfile => placementProfile;
  public ItemInteractionProfile InteractionProfile => interactionProfile;
  public IItemInteractionCapability InteractionCapability => interactionCapability;

  public abstract EItemCategory Category { get; }
  public abstract EItemRole Role { get; }
  public abstract int MaxStackSize { get; }

#if UNITY_EDITOR
  private void OnValidate()
  {
    itemName = name;
  }
#endif
}