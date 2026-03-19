using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject,
  IItemIconProvider, IItemDefinitionProvider
{
  [SerializeField]
  private List<ItemDefinition> itemDefinitions = new();

  [SerializeField]
  private List<IconProviderData> itemIcons = new();

  [Serializable]
  public class IconProviderData
  {
    public ItemKey Key;
    public Sprite Icon;
  }

  private Dictionary<int, ItemDefinition> _itemLookup;
  private Dictionary<int, Sprite> _spriteLookup;

  private void OnEnable()
  {
    BuildIconLookup();
    BuildItemLookup();
  }

  private void BuildIconLookup()
  {
    _spriteLookup = new Dictionary<int, Sprite>(itemIcons.Count);

    foreach (var item in itemIcons)
    {
      if (item == null)
        continue;

      int hashId = item.Key.RuntimeTag.Hash;

      if (!_spriteLookup.ContainsKey(hashId))
      {
        _spriteLookup.Add(hashId, item.Icon);
      }
      else
      {
        Debug.LogWarning($"Duplicate ItemId detected: {hashId}");
      }
    }


  }

  private void BuildItemLookup()
  {
    _itemLookup = new Dictionary<int, ItemDefinition>();

    foreach (var item in itemDefinitions)
    {
      if (item == null)
        continue;

      if (!_itemLookup.ContainsKey(item.ID))
        _itemLookup.Add(item.ID, item);
    }
  }

  public Sprite GetIcon(int itemId)
  {
    if (_spriteLookup == null)
      BuildIconLookup();

    if (_spriteLookup.TryGetValue(itemId, out var sprite))
      return sprite;

    Debug.LogWarning($"Icon not found for ItemId: {itemId}");
    return null;
  }

  public IItemDefinition GetItem(int itemId)
  {
    if (_itemLookup == null)
      BuildItemLookup();

    _itemLookup.TryGetValue(itemId, out var item);
    return item;
  }

  public IEnumerable<IItemDefinition> GetAll()
  {
    if (_itemLookup == null)
      BuildItemLookup();

    return _itemLookup.Values;
  }
}