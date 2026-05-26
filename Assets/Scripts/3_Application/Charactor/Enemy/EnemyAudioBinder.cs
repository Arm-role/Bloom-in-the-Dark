#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Central binder — subscribe EnemyManager lifecycle แล้ว hook OnDamaged ของทุก enemy ที่ active
// IsDead → OnEnemyDeath, ไม่ตาย → OnEnemyHit. SFX เล่นแบบ 3D ที่ position ของ enemy
public sealed class EnemyAudioBinder : IDisposable
{
  private readonly EnemyManager _enemyManager;
  private readonly IAudioService? _audio;
  private readonly ICombatSoundConfig? _soundConfig;

  // Track handler per enemy — ต้อง keep reference เดียวกันตอน unsubscribe
  private readonly Dictionary<EnemyController, Action<CharacterDamageResult>> _handlers = new();
  private bool _disposed;

  public EnemyAudioBinder(
      EnemyManager enemyManager,
      IAudioService? audio,
      ICombatSoundConfig? soundConfig)
  {
    _enemyManager = enemyManager;
    _audio = audio;
    _soundConfig = soundConfig;

    _enemyManager.OnEnemyRegistered += HandleRegistered;
    _enemyManager.OnEnemyUnregistered += HandleUnregistered;

    // hook enemy ที่ register อยู่แล้วก่อน binder ถูกสร้าง (กรณี binder สร้างหลัง enemy แรก spawn)
    foreach (var enemy in _enemyManager.ActiveEnemies)
      HandleRegistered(enemy);
  }

  private void HandleRegistered(EnemyController enemy)
  {
    if (_handlers.ContainsKey(enemy)) return;

    Action<CharacterDamageResult> handler = result => HandleDamaged(enemy, result);
    _handlers[enemy] = handler;
    enemy.OnDamaged += handler;
  }

  private void HandleUnregistered(EnemyController enemy)
  {
    if (!_handlers.TryGetValue(enemy, out var handler)) return;
    enemy.OnDamaged -= handler;
    _handlers.Remove(enemy);
  }

  private void HandleDamaged(EnemyController enemy, CharacterDamageResult result)
  {
    var key = result.IsDead ? _soundConfig?.OnEnemyDeath : _soundConfig?.OnEnemyHit;
    if (_audio == null || key == null || enemy == null) return;

    _audio.PlaySFX(key, enemy.transform);
  }

  public void Dispose()
  {
    if (_disposed) return;

    _enemyManager.OnEnemyRegistered -= HandleRegistered;
    _enemyManager.OnEnemyUnregistered -= HandleUnregistered;

    foreach (var kv in _handlers)
    {
      if (kv.Key != null)
        kv.Key.OnDamaged -= kv.Value;
    }
    _handlers.Clear();
    _disposed = true;
  }
}
