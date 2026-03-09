using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject, IItemIconProvider
{
  [SerializeField]
  private List<IconProviderData> entries = new();

  [Serializable]
  public class IconProviderData
  {
    public ItemKey Key;
    public Sprite Icon;
  }

  private Dictionary<int, Sprite> _spriteLookup;

  private void OnEnable()
  {
    BuildLookup();
  }

  private void BuildLookup()
  {
    _spriteLookup = new Dictionary<int, Sprite>(entries.Count);

    foreach (var entry in entries)
    {
      if (entry == null)
        continue;

      int hashId = entry.Key.RuntimeTag.Hash;

      if (!_spriteLookup.ContainsKey(hashId))
      {
        _spriteLookup.Add(hashId, entry.Icon);
      }
      else
      {
        Debug.LogWarning($"Duplicate ItemId detected: {hashId}");
      }
    }
  }

  public Sprite GetIcon(int itemId)
  {
    if (_spriteLookup == null)
      BuildLookup();

    if (_spriteLookup.TryGetValue(itemId, out var sprite))
      return sprite;

    Debug.LogWarning($"Icon not found for ItemId: {itemId}");
    return null;
  }
}