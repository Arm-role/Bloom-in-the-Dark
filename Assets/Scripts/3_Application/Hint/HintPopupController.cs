#nullable enable

using System;
using UnityEngine;

// Orchestrator: lookup entry → pause game → show view → mark viewed
// On close: resume game + hide view
// External caller (H2 menu, H3 unlock binder, debug) เรียก Show(id) จะ trigger flow ครบ
public sealed class HintPopupController : IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintPopupView _view;

  private bool _isOpen;
  private float _prevTimeScale = 1f;
  private bool _disposed;

  public HintPopupController(
    IHintLibrary library,
    IHintState state,
    IHintPopupView view)
  {
    _library = library;
    _state = state;
    _view = view;

    _view.OnCloseRequested += HandleCloseRequested;
  }

  public bool IsOpen => _isOpen;

  // Open popup ของ entry id — null/unknown id → log + ignore
  public void Show(string id)
  {
#if UNITY_EDITOR
    Debug.Log($"[HintPopupController] Show called id='{id}' isOpen={_isOpen}");
#endif
    if (_isOpen) return; // กัน open ซ้อน

    var entry = _library.GetById(id);
    if (entry == null)
    {
#if UNITY_EDITOR
      Debug.LogWarning($"[HintPopupController] Hint id '{id}' not found in library");
#endif
      return;
    }

#if UNITY_EDITOR
    Debug.Log($"[HintPopupController] Showing entry title='{entry.Title}'");
#endif
    _isOpen = true;
    _prevTimeScale = Time.timeScale;
    Time.timeScale = 0f;

    _view.Show(entry);
    _state.MarkViewed(id);

    // ดูแล้ว = unlock ด้วย → entry นี้จะโผล่ใน Menu ครั้งถัดไป
    // (สำคัญสำหรับ welcome popup ที่ไม่ผ่าน HintUnlockBinder.TryUnlock)
    _state.Unlock(id);
  }

  public void Close()
  {
    if (!_isOpen) return;

    _isOpen = false;
    Time.timeScale = _prevTimeScale;
    _view.Hide();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _view.OnCloseRequested -= HandleCloseRequested;
    _disposed = true;

    // เผื่อกรณี Dispose ระหว่าง popup เปิด — restore timeScale กัน game ค้าง pause
    if (_isOpen)
      Time.timeScale = _prevTimeScale;
  }

  private void HandleCloseRequested() => Close();
}
