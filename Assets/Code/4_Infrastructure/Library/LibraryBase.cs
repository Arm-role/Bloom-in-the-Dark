using System.Collections.Generic;
using UnityEngine;

public abstract class LibraryBase<T> : ScriptableObject
{
  [SerializeField] private List<ObjectEntry<T>> entries;

  private Dictionary<int, T> _lookup;

  protected virtual void OnEnable()
  {
    BuildLookup();
  }
  private void BuildLookup()
  {
    _lookup = new Dictionary<int, T>(entries.Count);

    foreach (var e in entries)
    {
      int hashId = e.Key.RuntimeTag.Hash;

      if (_lookup.ContainsKey(hashId))
      {
        Debug.LogError($"Duplicate key {e.Key.name} in {name}");
        continue;
      }

      _lookup.Add(hashId, e.Addressable);
    }
  }

  public T Find(int id)
  {
    if (_lookup == null)
      BuildLookup();

    if (_lookup.TryGetValue(id, out var value))
      return value;

    Debug.LogError($"Key not found: {id}");
    return default;
  }
}
