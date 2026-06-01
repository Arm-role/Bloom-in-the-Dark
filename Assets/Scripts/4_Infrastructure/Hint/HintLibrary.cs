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
  [Header("Event trigger entry slots (drag HintEntry asset)")]
  [Tooltip("Forced popup ตอน gameplayState.Enter ครั้งแรก")]
  [SerializeField] private HintEntry? _welcomeEntry;

  [Tooltip("HP cross HealthLowThreshold ลงมาครั้งแรก — สอนวิธีฟื้น HP")]
  [SerializeField] private HintEntry? _damageEntry;

  [Tooltip("PlayerEnergy cross threshold ลงมาครั้งแรก — สอนวิธีเพิ่ม energy + reset")]
  [SerializeField] private HintEntry? _energyLowEntry;

  [Tooltip("เข้า Preparation phase ครั้งแรก")]
  [SerializeField] private HintEntry? _phasePrepareEntry;

  [Tooltip("เข้า Battle phase ครั้งแรก — สอนการใช้ skill")]
  [SerializeField] private HintEntry? _phaseBattleEntry;

  [Tooltip("ใช้ item ทำ action สำเร็จครั้งแรก (click ซ้าย/ขวา) — UI click ไม่ trigger")]
  [SerializeField] private HintEntry? _actionHintEntry;

  [Tooltip("ได้พืช upgradable ครบ N ลูก — สอน altar upgrade")]
  [SerializeField] private HintEntry? _plantCountEntry;

  [Tooltip("ได้กิ่งไม้ + ก้อนหิน — สอน altar craft")]
  [SerializeField] private HintEntry? _branchStoneComboEntry;

  [Header("Detection slots — ItemTag refs")]
  [SerializeField] private ItemTag? _upgradablePlantTag;
  [SerializeField] private ItemTag? _branchTag;
  [SerializeField] private ItemTag? _stoneTag;

  [Header("Config")]
  [Tooltip("Energy ≤ ค่านี้ → fire EnergyLowEntry ครั้งแรก (default 30)")]
  [SerializeField] private float _energyLowThreshold = 30f;

  [Tooltip("HP ≤ ค่านี้ → fire DamageEntry ครั้งแรก (default 60)")]
  [SerializeField] private float _healthLowThreshold = 60f;

  [Tooltip("จำนวนพืช upgradable สะสมที่ต้องการ → fire PlantCountEntry (default 5)")]
  [SerializeField] private int _plantCountThreshold = 5;

  private readonly List<IHintEntry> _combinedCache = new();
  private bool _combinedDirty = true;

  public IHintEntry? WelcomeEntry => _welcomeEntry;
  public IHintEntry? DamageEntry => _damageEntry;
  public IHintEntry? EnergyLowEntry => _energyLowEntry;
  public IHintEntry? PhasePrepareEntry => _phasePrepareEntry;
  public IHintEntry? PhaseBattleEntry => _phaseBattleEntry;
  public IHintEntry? ActionHintEntry => _actionHintEntry;
  public IHintEntry? PlantCountEntry => _plantCountEntry;
  public IHintEntry? BranchStoneComboEntry => _branchStoneComboEntry;

  public ItemTag? UpgradablePlantTag => _upgradablePlantTag;
  public ItemTag? BranchTag => _branchTag;
  public ItemTag? StoneTag => _stoneTag;

  public float EnergyLowThreshold => _energyLowThreshold;
  public float HealthLowThreshold => _healthLowThreshold;
  public int PlantCountThreshold => _plantCountThreshold;

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
    TryAdd(_energyLowEntry);
    TryAdd(_phasePrepareEntry);
    TryAdd(_phaseBattleEntry);
    TryAdd(_actionHintEntry);
    TryAdd(_plantCountEntry);
    TryAdd(_branchStoneComboEntry);
    _combinedDirty = false;
  }

  private void TryAdd(HintEntry? entry)
  {
    if (entry == null || string.IsNullOrEmpty(entry.Id)) return;
    if (_combinedCache.Contains(entry)) return; // dedup
    _combinedCache.Add(entry);
  }
}
