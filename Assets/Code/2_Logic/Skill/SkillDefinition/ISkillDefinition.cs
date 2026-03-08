public interface ISkillDefinition
{
  public string SkillId { get; }
  public bool Execute(IItemInstance instance, out ISkillDataPayload payload);
}