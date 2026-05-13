using System.Collections.Generic;

public static class TagLibrary
{
  private static readonly Dictionary<string, GameTag> _tags = new();

  public static void Initialize(ITagLibraryAsset asset)
  {
    _tags.Clear();

    foreach (var tagAsset in asset.Tags)
    {
      if (tagAsset == null) continue;

      string id = tagAsset.name;

      if (!_tags.ContainsKey(id))
        _tags.Add(id, tagAsset.RuntimeTag);
    }
  }

  public static GameTag Get(string id)
  {
    if (_tags.TryGetValue(id, out var tag))
      return tag;

    UnityEngine.Debug.LogError($"Tag not found: {id}");
    return default;
  }
}