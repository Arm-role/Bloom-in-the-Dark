#nullable enable

using System;
using UnityEngine;

// Subscribe HotbarState.OnSlotChanged → PlaySFX
// Pure C# IDisposable — RuntimeInstaller สร้างและถือ reference ตลอด lifetime ของ scene
// ถ้า _audio หรือ key เป็น null → silent (no-op)
public sealed class HotbarAudioBinder : IDisposable
{
  private readonly HotbarState _hotbarState;
  private readonly IAudioService? _audio;
  private readonly IInventorySoundConfig? _soundConfig;
  private bool _disposed;

  public HotbarAudioBinder(
      HotbarState hotbarState,
      IAudioService? audio,
      IInventorySoundConfig? soundConfig)
  {
    _hotbarState = hotbarState;
    _audio = audio;
    _soundConfig = soundConfig;

    _hotbarState.OnSlotChanged += HandleSlotChanged;
  }

  private void HandleSlotChanged(int index)
  {
    var key = _soundConfig?.OnHotbarSelect;
    if (_audio == null || key == null) return;

    _audio.PlaySFX(key, Vector3.zero);
  }

  public void Dispose()
  {
    if (_disposed) return;
    _hotbarState.OnSlotChanged -= HandleSlotChanged;
    _disposed = true;
  }
}
