#nullable enable

using System;
using UnityEngine;

// Subscribe PlayerController combat events → PlaySFX (3D ที่ player position)
// Pure C# IDisposable — RuntimeInstaller สร้าง + ถือ reference ตลอด scene
public sealed class PlayerAudioBinder : IDisposable
{
  private readonly PlayerController _player;
  private readonly IAudioService? _audio;
  private readonly ICombatSoundConfig? _soundConfig;
  private bool _disposed;

  public PlayerAudioBinder(
      PlayerController player,
      IAudioService? audio,
      ICombatSoundConfig? soundConfig)
  {
    _player = player;
    _audio = audio;
    _soundConfig = soundConfig;

    _player.OnDamaged += HandleDamaged;
    _player.OnPlayerDied += HandleDied;
    _player.OnHeal += HandleHeal;
  }

  private void HandleDamaged(CharacterDamageResult result)
    => PlayAt(_soundConfig?.OnPlayerHit, _player.transform);

  private void HandleDied()
    => PlayAt(_soundConfig?.OnPlayerDeath, _player.transform);

  private void HandleHeal(PlayerHealthResult result)
    => PlayAt(_soundConfig?.OnPlayerHeal, _player.transform);

  private void PlayAt(SoundKey? key, Transform follow)
  {
    if (_audio == null || key == null) return;
    _audio.PlaySFX(key, follow);
  }

  public void Dispose()
  {
    if (_disposed) return;
    _player.OnDamaged -= HandleDamaged;
    _player.OnPlayerDied -= HandleDied;
    _player.OnHeal -= HandleHeal;
    _disposed = true;
  }
}
