public interface ISkillDefinition
{
  int SkillId { get; }
  float GetBaseStat(StatKey key);
  bool Execute(IItemInstance instance, out ISkillDataPayload payload);
}