#nullable enable

using System;
using UnityEngine;

// Data-driven mapping: HintUnlockBinder อ่าน entry ของแต่ละ event จาก HintLibrary
// → ไม่มี hardcoded const ID — designer ตั้งใน Library inspector
//
// Subscribe ใน ctor (รัน scene ทั้งหมด), check IsUnlocked ก่อน fire (idempotent per event)
// IGameSystem.Enter → trigger welcome popup (ครั้งเดียวต่อ save ตาม IsWelcomeShown flag)
//
// Current events:
//   - gameplayState.Enter → welcome
//   - PlayerController.OnDamaged → damage
// (D2/D4 จะเพิ่ม turn-state, energy-threshold, item-detection handlers ทีหลัง)
public sealed class HintUnlockBinder : IGameSystem, IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly HintPopupController _popup;
  private readonly PlayerController _player;

  private bool _disposed;

  public HintUnlockBinder(
    IHintLibrary library,
    IHintState state,
    HintPopupController popup,
    PlayerController player)
  {
    _library = library;
    _state = state;
    _popup = popup;
    _player = player;

    _player.OnDamaged += HandlePlayerDamaged;
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
    _player.OnDamaged -= HandlePlayerDamaged;
    _disposed = true;
  }

  // ==========================
  // Event handlers
  // ==========================

  private void HandlePlayerDamaged(CharacterDamageResult result)
    => TryUnlockEntry(_library.DamageEntry);

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
