#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Orchestrator สำหรับ Hint Menu:
//   - Toggle ปิด/เปิด ผ่าน view.OnToggleRequested (กด H)
//   - Open → filter library entries → pause game → view.ShowMenu
//   - Entry clicked → delegate ไป HintPopupController.Show (popup ทับบน menu)
//   - Close → resume game → view.Hide
public sealed class HintMenuController : IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintMenuView _view;
  private readonly HintPopupController _popup;

  private bool _isOpen;
  private float _prevTimeScale = 1f;
  private bool _disposed;

  public HintMenuController(
    IHintLibrary library,
    IHintState state,
    IHintMenuView view,
    HintPopupController popup)
  {
    _library = library;
    _state = state;
    _view = view;
    _popup = popup;

    _view.OnToggleRequested += HandleToggle;
    _view.OnCloseRequested += HandleClose;
    _view.OnEntryClicked += HandleEntryClicked;
  }

  public bool IsOpen => _isOpen;

  public void Toggle()
  {
    if (_isOpen) Close();
    else Open();
  }

  public void Open()
  {
    if (_isOpen) return;

    _isOpen = true;
    _prevTimeScale = Time.timeScale;
    Time.timeScale = 0f;

    var entries = BuildVisibleEntries();
    _view.ShowMenu(entries);
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
    _view.OnToggleRequested -= HandleToggle;
    _view.OnCloseRequested -= HandleClose;
    _view.OnEntryClicked -= HandleEntryClicked;
    _disposed = true;

    if (_isOpen)
      Time.timeScale = _prevTimeScale;
  }

  // ==========================
  // Internal
  // ==========================

  // คืน entries ที่ player ควรเห็น — UnlockedByDefault เห็นเสมอ + ที่ state ปลดล็อกแล้ว
  private List<IHintEntry> BuildVisibleEntries()
  {
    var list = new List<IHintEntry>();
    foreach (var entry in _library.Entries)
    {
      if (entry.UnlockedByDefault || _state.IsUnlocked(entry.Id))
        list.Add(entry);
    }
    return list;
  }

  // ถ้า popup เปิดอยู่ → ignore toggle (กัน H ปิด menu ขณะ popup ทับ → timeScale stack จะพัง)
  private void HandleToggle()
  {
    if (_popup.IsOpen) return;
    Toggle();
  }

  private void HandleClose()
  {
    // กัน close menu ขณะ popup เปิด — popup ต้องปิดก่อน
    if (_popup.IsOpen) return;
    Close();
  }

  // popup โผล่ทับ menu (เกม pause อยู่จากทั้งคู่ — timeScale stack จัดการได้)
  private void HandleEntryClicked(string id) => _popup.Show(id);
}
