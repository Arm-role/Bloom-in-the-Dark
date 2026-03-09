public interface IItemDefinition
{
  public int ID { get; }
  public string Name { get; }
  public GameTag Key { get; }
  public GameTagContainer CreateTagContainer();

  public GameTagContainer Tags{ get; }
  public bool HasTag(GameTag tag);

  EItemRole Role { get; }
  int MaxStackSize { get; }

  ISkillDefinition Skill { get; }
  IPlacementProfile PlacementProfile { get; }
  IItemInteractionProfile InteractionProfile { get; }
  IItemInteractionCapability InteractionCapability { get; }
}