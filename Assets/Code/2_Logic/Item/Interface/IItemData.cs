using UnityEngine;

public interface IItemData
{
  int ID { get; }
  string Name { get; }
  Sprite Icon { get; }

  ISkillDefinition Skill { get; }
  IPlacementProfile PlacementProfile { get; }
  IItemInteractionProfile InteractionProfile { get; }
  IItemInteractionCapability InteractionCapability { get; }

  EItemCategory Category { get; }
  EItemRole Role { get; }
  int MaxStackSize { get; }
}