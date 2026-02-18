using UnityEngine;

public interface IItemData
{
    int ID { get; }
    string Name { get; }
    Sprite Icon { get; }
   
    SkillDefinition Skill { get; }
    PlacementProfile PlacementProfile { get; }
    ItemInteractionProfile InteractionProfile { get; }
    IItemInteractionCapability InteractionCapability { get; }
    
    EItemCategory Category { get; }
    EItemRole Role { get; }
    int MaxStackSize { get; }
}