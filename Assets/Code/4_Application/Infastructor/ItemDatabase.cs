using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject, IItemIconProvider
{
  [SerializeField]
  private List<Item> entries = new();

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

      if (!_spriteLookup.ContainsKey(entry.ID))
      {
        _spriteLookup.Add(entry.ID, entry.Icon);
      }
      else
      {
        Debug.LogWarning($"Duplicate ItemId detected: {entry.ID}");
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

public interface IItemIconProvider
{
  Sprite GetIcon(int itemId);
}