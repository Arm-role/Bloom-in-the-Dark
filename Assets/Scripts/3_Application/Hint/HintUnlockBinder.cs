#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Hard-coded mapping ระหว่าง game event → hint id
// Subscribe ใน ctor (รัน scene ทั้งหมด), check IsUnlocked ก่อน fire (idempotent per event)
// IGameSystem.Enter → trigger welcome popup (ครั้งเดียวต่อ save ตาม IsWelcomeShown flag)
public sealed class HintUnlockBinder : IGameSystem, IDisposable
{
  // Hint IDs — designer ต้องสร้าง HintEntry SO ที่มี Id ตรงตามนี้
  private const string ID_WELCOME = "welcome";
  private const string ID_INTRO_PICKUP = "intro_pickup";
  private const string ID_INTRO_COMBAT = "intro_combat";
  private const string ID_INTRO_DAMAGE = "intro_damage";

  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly HintPopupController _popup;

  // Event sources
  private readonly PlayerInventory _inventory;
  private readonly PlayerController _player;
  private readonly EnemyManager? _enemyManager;

  // Track per-enemy OnDamaged handlers — unsubscribe ตอน enemy unregister + ตอน dispose
  private readonly Dictionary<EnemyController, Action<CharacterDamageResult>> _enemyHandlers = new();
  private bool _disposed;

  public HintUnlockBinder(
    IHintLibrary library,
    IHintState state,
    HintPopupController popup,
    PlayerInventory inventory,
    PlayerController player,
    EnemyManager? enemyManager)
  {
    _library = library;
    _state = state;
    _popup = popup;
    _inventory = inventory;
    _player = player;
    _enemyManager = enemyManager;

    SubscribeAll();
  }

  // ==========================
  // IGameSystem — welcome trigger
  // ==========================

  public void Enter()
  {
#if UNITY_EDITOR
    Debug.Log($"[HintUnlockBinder] Enter — IsWelcomeShown={_state.IsWelcomeShown}");
#endif
    if (_state.IsWelcomeShown) return;

    // ตรวจ entry ก่อน mark — กัน flag ตั้งทั้งที่ popup fail
    // (ถ้า designer ยังไม่สร้าง entry → bail out, flag ยังไม่ตั้ง, ครั้งหน้าลองใหม่)
    if (_library.GetById(ID_WELCOME) == null)
    {
#if UNITY_EDITOR
      Debug.LogWarning($"[HintUnlockBinder] Welcome entry id='{ID_WELCOME}' missing in library — skip");
#endif
      return;
    }

    _state.MarkWelcomeShown();
#if UNITY_EDITOR
    Debug.Log($"[HintUnlockBinder] Triggering welcome popup id='{ID_WELCOME}'");
#endif
    _popup.Show(ID_WELCOME);
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
    UnsubscribeAll();
    _disposed = true;
  }

  // ==========================
  // Subscriptions
  // ==========================

  private void SubscribeAll()
  {
    _inventory.Hotbar.OnItemAdded += HandleItemAdded;
    _inventory.MainInventory.OnItemAdded += HandleItemAdded;

    _player.OnDamaged += HandlePlayerDamaged;

    if (_enemyManager != null)
    {
      _enemyManager.OnEnemyRegistered += HandleEnemyRegistered;
      _enemyManager.OnEnemyUnregistered += HandleEnemyUnregistered;

      // hook enemies ที่ register อยู่แล้ว (กรณี binder สร้างหลัง enemy แรก spawn)
      foreach (var e in _enemyManager.ActiveEnemies)
        HandleEnemyRegistered(e);
    }
  }

  private void UnsubscribeAll()
  {
    _inventory.Hotbar.OnItemAdded -= HandleItemAdded;
    _inventory.MainInventory.OnItemAdded -= HandleItemAdded;

    _player.OnDamaged -= HandlePlayerDamaged;

    if (_enemyManager != null)
    {
      _enemyManager.OnEnemyRegistered -= HandleEnemyRegistered;
      _enemyManager.OnEnemyUnregistered -= HandleEnemyUnregistered;
    }

    foreach (var kv in _enemyHandlers)
    {
      if (kv.Key != null)
        kv.Key.OnDamaged -= kv.Value;
    }
    _enemyHandlers.Clear();
  }

  // ==========================
  // Event handlers
  // ==========================

  private void HandleItemAdded(IItemDefinition data, int amount)
    => TryUnlock(ID_INTRO_PICKUP);

  private void HandlePlayerDamaged(CharacterDamageResult result)
    => TryUnlock(ID_INTRO_DAMAGE);

  private void HandleEnemyRegistered(EnemyController enemy)
  {
    if (_enemyHandlers.ContainsKey(enemy)) return;

    Action<CharacterDamageResult> handler = _ => TryUnlock(ID_INTRO_COMBAT);
    _enemyHandlers[enemy] = handler;
    enemy.OnDamaged += handler;
  }

  private void HandleEnemyUnregistered(EnemyController enemy)
  {
    if (!_enemyHandlers.TryGetValue(enemy, out var handler)) return;
    enemy.OnDamaged -= handler;
    _enemyHandlers.Remove(enemy);
  }

  // ==========================
  // Helper
  // ==========================

  // unlock ครั้งแรกเท่านั้น — ครั้งถัดไป no-op (idempotent)
  // ไม่มี toast notify — player ต้องเปิด menu (H) มาดูเอง
  private void TryUnlock(string id)
  {
    if (_state.IsUnlocked(id)) return;
    _state.Unlock(id);

#if UNITY_EDITOR
    var entry = _library.GetById(id);
    Debug.Log($"[HintUnlockBinder] Unlocked id='{id}' title='{entry?.Title ?? "<missing>"}'");
#endif
  }
}
