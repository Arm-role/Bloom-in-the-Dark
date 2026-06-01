#nullable enable

using System;
using UnityEngine;

// Data-driven mapping: HintUnlockBinder อ่าน entry ของแต่ละ event จาก HintLibrary
// → ไม่มี hardcoded const ID — designer ตั้งใน Library inspector
//
// Subscribe ใน ctor (รัน scene ทั้งหมด), check IsUnlocked ก่อน fire (idempotent per event)
// IGameSystem.Enter → trigger welcome popup (ครั้งเดียวต่อ save ตาม IsWelcomeShown flag)
//
// Events:
//   D1: gameplayState.Enter → welcome
//   D2: PlayerHealth.OnChanged cross HealthLowThreshold → damage hint (HP ตกถึงเกณฑ์ครั้งแรก)
//       PlayerEnergy.OnChanged cross EnergyLowThreshold → energy low
//       ITurnSystem.OnTurnTransitionComplete (Preparation/Battle) → phase hints
//                — รอ transition canvas เล่นจบก่อนค่อยยิง popup (กันชน + กัน timeScale=0 ค้าง transition)
//   D4: PlayerInventory.OnItemAdded → plant counter (tag) / branch+stone combo (2 tags)
//       ItemInteractionAction.OnActionCommitted → action hint (item action สำเร็จครั้งแรก)
//                — late-bound ผ่าน HookActionSource ใน installer เพราะ HintUnlockBinder ถูกสร้างก่อน
//                  ItemInteractionAction (คนละ installer)
public sealed class HintUnlockBinder : IGameSystem, IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly HintPopupController _popup;
  private readonly PlayerHealth _playerHealth;
  private readonly PlayerEnergy _energy;
  private readonly ITurnSystem _turnSystem;
  private readonly PlayerInventory _inventory;

  // threshold-cross detection
  private float _lastEnergy = float.MaxValue;
  private float _lastHealth = float.MaxValue;

  // upgradable plant counter (reset ทุก scene — ตามที่ user ตัดสิน)
  private int _plantCount;

  // late-bound action source — hook ผ่าน HookActionSource() หลัง ItemInteractionAction ถูกสร้าง
  private ItemInteractionAction? _itemInteraction;

  private bool _disposed;

  public HintUnlockBinder(
    IHintLibrary library,
    IHintState state,
    HintPopupController popup,
    PlayerHealth playerHealth,
    PlayerEnergy energy,
    ITurnSystem turnSystem,
    PlayerInventory inventory)
  {
    _library = library;
    _state = state;
    _popup = popup;
    _playerHealth = playerHealth;
    _energy = energy;
    _turnSystem = turnSystem;
    _inventory = inventory;

    _playerHealth.OnChanged += HandleHealthChanged;
    _energy.OnChanged += HandleEnergyChanged;
    // subscribe transition-complete (ไม่ใช่ OnNextTurn) — กัน popup ทับกับ turn transition canvas
    _turnSystem.OnTurnTransitionComplete += HandleTurnChanged;
    _inventory.Hotbar.OnItemAdded += HandleItemAdded;
    _inventory.MainInventory.OnItemAdded += HandleItemAdded;
  }

  // ==========================
  // IGameSystem — welcome trigger
  // ==========================

  public void Enter()
  {
    var welcome = _library.WelcomeEntry;
#if UNITY_EDITOR
    Debug.Log($"[HintUnlockBinder] Enter — IsWelcomeShown={_state.IsWelcomeShown}, WelcomeEntry={(welcome?.Id ?? "<null>")}");
#endif
    if (_state.IsWelcomeShown) return;

    if (welcome == null)
    {
#if UNITY_EDITOR
      Debug.LogWarning("[HintUnlockBinder] WelcomeEntry slot in HintLibrary is empty — skip");
#endif
      return;
    }

    _state.MarkWelcomeShown();
#if UNITY_EDITOR
    Debug.Log($"[HintUnlockBinder] Triggering welcome popup id='{welcome.Id}'");
#endif
    _popup.Show(welcome.Id);
  }

  public void Exit() { }
  public void Update(float dt) { }
  public void FixedUpdate(float dt) { }

  // ==========================
  // IDisposable
  // ==========================

  public void Dispose()
  {
    if (_disposed) return;
    _playerHealth.OnChanged -= HandleHealthChanged;
    _energy.OnChanged -= HandleEnergyChanged;
    _turnSystem.OnTurnTransitionComplete -= HandleTurnChanged;
    _inventory.Hotbar.OnItemAdded -= HandleItemAdded;
    _inventory.MainInventory.OnItemAdded -= HandleItemAdded;
    if (_itemInteraction != null)
      _itemInteraction.OnActionCommitted -= HandleActionCommitted;
    _disposed = true;
  }

  // ==========================
  // Late binding — ItemInteractionAction (สร้างใน SceneBindingInstaller หลัง HintUnlockBinder)
  // ==========================

  // Hook ครั้งเดียวเท่านั้น — เรียกซ้ำ no-op (กันรับ event ซ้ำถ้า installer logic เปลี่ยน)
  public void HookActionSource(ItemInteractionAction source)
  {
    if (_itemInteraction != null) return;
    _itemInteraction = source;
    _itemInteraction.OnActionCommitted += HandleActionCommitted;
  }

  private void HandleActionCommitted()
    => TryUnlockEntry(_library.ActionHintEntry);

  // ==========================
  // Event handlers — D2
  // ==========================

  private void HandleHealthChanged(ResourceChangedEvent e)
  {
    var threshold = _library.HealthLowThreshold;
    bool wasAbove = _lastHealth > threshold;
    bool nowBelow = e.Current <= threshold;
    _lastHealth = e.Current;

    if (wasAbove && nowBelow)
      TryUnlockEntry(_library.DamageEntry);
  }

  private void HandleEnergyChanged(ResourceChangedEvent e)
  {
    var threshold = _library.EnergyLowThreshold;
    bool wasAbove = _lastEnergy > threshold;
    bool nowBelow = e.Current <= threshold;
    _lastEnergy = e.Current;

    if (wasAbove && nowBelow)
      TryUnlockEntry(_library.EnergyLowEntry);
  }

  private void HandleTurnChanged(ETurnState state)
  {
    switch (state)
    {
      case ETurnState.Preparation:
        TryUnlockEntry(_library.PhasePrepareEntry);
        break;
      case ETurnState.Battle:
        TryUnlockEntry(_library.PhaseBattleEntry);
        break;
    }
  }

  // ==========================
  // Event handlers — D4
  // ==========================

  // OnItemAdded — handle หลาย hint ใน handler เดียว
  private void HandleItemAdded(IItemDefinition data, int amount)
  {
    // Upgradable plant counter
    var plantTag = _library.UpgradablePlantTag;
    if (plantTag != null && data.HasTag(plantTag.RuntimeTag))
    {
      _plantCount += amount;
      if (_plantCount >= _library.PlantCountThreshold)
        TryUnlockEntry(_library.PlantCountEntry);
    }

    // Branch + Stone combo (ตรวจ inventory ทั้งหมด — ถ้ามีทั้ง 2 → unlock)
    CheckBranchStoneCombo();
  }

  private void CheckBranchStoneCombo()
  {
    var comboEntry = _library.BranchStoneComboEntry;
    if (comboEntry == null) return;
    if (_state.IsUnlocked(comboEntry.Id)) return; // early bail

    var branchTag = _library.BranchTag;
    var stoneTag = _library.StoneTag;
    if (branchTag == null || stoneTag == null) return;

    if (InventoryHasTag(branchTag) && InventoryHasTag(stoneTag))
      TryUnlockEntry(comboEntry);
  }

  private bool InventoryHasTag(ItemTag tag)
  {
    var runtimeTag = tag.RuntimeTag;
    foreach (var slot in _inventory.Hotbar.Slots)
    {
      if (slot.IsEmpty) continue;
      if (slot.GetItemInstance()!.Data.HasTag(runtimeTag)) return true;
    }
    foreach (var slot in _inventory.MainInventory.Slots)
    {
      if (slot.IsEmpty) continue;
      if (slot.GetItemInstance()!.Data.HasTag(runtimeTag)) return true;
    }
    return false;
  }

  // ==========================
  // Helper
  // ==========================

  // unlock + auto-show ครั้งแรกเท่านั้น — entry ไม่มี / unlock แล้ว → no-op
  private void TryUnlockEntry(IHintEntry? entry)
  {
    if (entry == null) return;
    if (_state.IsUnlocked(entry.Id)) return;

    _state.Unlock(entry.Id);

#if UNITY_EDITOR
    Debug.Log($"[HintUnlockBinder] Unlocked id='{entry.Id}' title='{entry.Title}' autoShow={entry.AutoShowOnUnlock}");
#endif

    if (entry.AutoShowOnUnlock)
      _popup.Show(entry.Id);
  }
}
