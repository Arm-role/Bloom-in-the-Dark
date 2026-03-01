using UnityEngine;

public abstract class SkillDefinition : ScriptableObject
{
  public abstract bool Execute(IItemInstance instance, out ISkillDataPayload payload);
}