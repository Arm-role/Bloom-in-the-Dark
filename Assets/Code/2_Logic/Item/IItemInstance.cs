using System.Collections.Generic;

public interface IItemInstance
{
  IItemDefinition Data { get; }
  int Level { get; }
  IEnumerable<StatModifier> GetModifiers();
  float GetStat(StatKey key);
}