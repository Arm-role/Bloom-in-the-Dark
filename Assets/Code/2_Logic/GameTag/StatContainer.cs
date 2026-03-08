using System.Collections.Generic;

public class StatContainer
{
  private Dictionary<GameTag, float> _stats = new();

  public void Add(GameTag tag, float value)
  {
    _stats[tag] = value;
  }

  public float Get(GameTag tag)
  {
    return _stats.TryGetValue(tag, out var v) ? v : 0;
  }
}