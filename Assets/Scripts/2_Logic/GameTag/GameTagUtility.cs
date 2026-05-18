public static class GameTagUtility
{
  public static bool Matches(GameTagAsset tag, GameTagAsset target)
  {
    while (tag != null)
    {
      if (tag == target)
        return true;

      tag = tag.Parent;
    }

    return false;
  }
}