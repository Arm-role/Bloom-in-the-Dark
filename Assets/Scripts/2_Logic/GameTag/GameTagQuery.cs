using System.Collections.Generic;

public class GameTagQuery
{
  private readonly List<GameTag> _required = new();
  private readonly List<GameTag> _blocked = new();

  public void Require(GameTag tag)
  {
    _required.Add(tag);
  }

  public void Block(GameTag tag)
  {
    _blocked.Add(tag);
  }

  public bool Matches(GameTagContainer container)
  {
    foreach (var tag in _required)
      if (!container.Has(tag))
        return false;

    foreach (var tag in _blocked)
      if (container.Has(tag))
        return false;

    return true;
  }
}