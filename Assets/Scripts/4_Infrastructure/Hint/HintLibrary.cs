#nullable enable

using System.Collections.Generic;
using UnityEngine;

// Library SO — designer ลำดับ HintEntry ทุกตัวของเกมที่นี่
// Lookup ใช้ Dictionary build lazy ตอน GetById ครั้งแรก (สอดคล้องกับ AudioLibrary pattern)
[CreateAssetMenu(menuName = "Hint/Hint Library")]
public sealed class HintLibrary : ScriptableObject, IHintLibrary
{
  [SerializeField] private List<HintEntry> _entries = new();

  private Dictionary<string, IHintEntry>? _lookup;

  // covariance: List<HintEntry> upcast เป็น IReadOnlyList<IHintEntry> ได้ (out T)
  public IReadOnlyList<IHintEntry> Entries => _entries;

  public IHintEntry? GetById(string id)
  {
    if (_lookup == null) BuildLookup();
    _lookup!.TryGetValue(id, out var entry);
    return entry;
  }

  private void OnEnable() => BuildLookup();

  private void BuildLookup()
  {
    _lookup = new Dictionary<string, IHintEntry>();
    foreach (var entry in _entries)
    {
      if (entry == null || string.IsNullOrEmpty(entry.Id)) continue;
      if (_lookup.ContainsKey(entry.Id))
      {
#if UNITY_EDITOR
        Debug.LogWarning($"[HintLibrary] Duplicate hint id '{entry.Id}' — first wins");
#endif
        continue;
      }
      _lookup[entry.Id] = entry;
    }
  }
}
