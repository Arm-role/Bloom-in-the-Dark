using UnityEngine;

public abstract class SkillDefinition : ScriptableObject, ISkillDefinition
{
  public abstract string SkillId { get; }
  public abstract bool Execute(IItemInstance instance, out ISkillDataPayload payload);
}