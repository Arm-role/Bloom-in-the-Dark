using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDefinition : ScriptableObject, ISkillDefinition
{
  protected Dictionary<StatKey, float> _baseStats;
  public abstract string SkillId { get; }

  protected virtual void OnEnable()
  {
    BuildStatMap();
  }
  protected abstract void BuildStatMap();
  public float GetBaseStat(StatKey key)
  {
    if (_baseStats == null)
      BuildStatMap();

    return _baseStats.TryGetValue(key, out var v) ? v : 0f;
  }

  public abstract bool Execute(IItemInstance instance, out ISkillDataPayload payload);
}