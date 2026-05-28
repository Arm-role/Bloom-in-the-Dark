#nullable enable

using System.Collections.Generic;
using UnityEngine;

// Library SO — config style:
//   Designer ลาก HintEntry ลง slot เท่านั้น — ไม่ต้อง maintain _entries list แยก
//   Entries property รวม slot ทั้งหมด (non-null, deduped) สำหรับ menu listing
//
// เพิ่ม slot ใหม่: เพิ่ม SerializeField + getter + เพิ่ม TryAdd ใน BuildCombined
[CreateAssetMenu(menuName = "Hint/Hint Library")]
public sealed class HintLibrary : ScriptableObject, IHintLibrary
{
  [Header("Event trigger slots (drag HintEntry asset)")]
  [Tooltip("Forced popup ตอน gameplayState.Enter ครั้งแรก")]
  [SerializeField] private HintEntry? _welcomeEntry;

  [Tooltip("PlayerController.OnDamaged ครั้งแรก (player ถูก hit)")]
  [SerializeField] private HintEntry? _damageEntry;

  private readonly List<IHintEntry> _combinedCache = new();
  private bool _combinedDirty = true;

  public IHintEntry? WelcomeEntry => _welcomeEntry;
  public IHintEntry? DamageEntry => _damageEntry;

  public IReadOnlyList<IHintEntry> Entries
  {
    get
    {
      if (_combinedDirty)
        BuildCombined();
      return _combinedCache;
    }
  }

  public IHintEntry? GetById(string id)
  {
    foreach (var entry in Entries)
      if (entry.Id == id)
        return entry;
    return null;
  }

  private void OnEnable() => _combinedDirty = true;

#if UNITY_EDITOR
  private void OnValidate() => _combinedDirty = true;
#endif

  private void BuildCombined()
  {
    _combinedCache.Clear();
    TryAdd(_welcomeEntry);
    TryAdd(_damageEntry);
    _combinedDirty = false;
  }

  private void TryAdd(HintEntry? entry)
  {
    if (entry == null || string.IsNullOrEmpty(entry.Id)) return;
    if (_combinedCache.Contains(entry)) return; // dedup
    _combinedCache.Add(entry);
  }
}
