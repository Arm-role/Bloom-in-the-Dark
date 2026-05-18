using System.Collections.Generic;

public class GameTagContainer
{
  private readonly HashSet<GameTag> _tags = new();

  public void Add(GameTag tag)
  {
    _tags.Add(tag);
  }

  public bool Has(GameTag tag)
  {
    return _tags.Contains(tag);
  }

  public bool HasAny(IEnumerable<GameTag> tags)
  {
    foreach (var tag in tags)
      if (_tags.Contains(tag))
        return true;

    return false;
  }

  public bool HasAll(IEnumerable<GameTag> tags)
  {
    foreach (var tag in tags)
      if (!_tags.Contains(tag))
        return false;

    return true;
  }
}
