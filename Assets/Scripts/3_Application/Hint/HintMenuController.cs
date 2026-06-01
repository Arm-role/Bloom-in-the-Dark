#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Book-style orchestrator (flat page list — no tabs):
//   - Open → filter unlocked entries → push modal stack → ShowMenu → ShowPage 0
//   - Prev/Next → clamp pageIndex → ShowPage
//   - Close → pop stack → Hide
//
// บทบาทคู่:
//   HintPopupController = forced reading (welcome / AutoShowOnUnlock)
//   HintMenuController  = browse mode (book + page navigation)
//
// Toggle gate: เปิดเฉพาะตอน Gameplay state + ไม่มี modal อื่น (กัน menu ทับ popup/inventory/upgrade/pause/trade)
public sealed class HintMenuController : IModalUI, IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintMenuView _view;
  private readonly GameStateMachine _stateMachine;
  private readonly ModalUIStack _modalStack;

  // Flat list — ตามลำดับใน Library (UnlockedByDefault + unlocked entries)
  private readonly List<IHintEntry> _visibleEntries = new();

  private bool _isOpen;
  private int _pageIndex;
  private bool _disposed;

  public HintMenuController(
    IHintLibrary library,
    IHintState state,
    IHintMenuView view,
    GameStateMachine stateMachine,
    ModalUIStack modalStack)
  {
    _library = library;
    _state = state;
    _view = view;
    _stateMachine = stateMachine;
    _modalStack = modalStack;

    _view.OnToggleRequested += HandleToggle;
    _view.OnCloseRequested += HandleClose;
    _view.OnPrevPageRequested += HandlePrevPage;
    _view.OnNextPageRequested += HandleNextPage;
  }

  // ==========================
  // IModalUI
  // ==========================

  public bool IsOpen => _isOpen;
  public bool PausesGame => true;
  public void HandleDismiss() => Close();

  // ==========================
  // Public API
  // ==========================

  public void Toggle()
  {
    if (_isOpen) { Close(); return; }

    // เปิดได้จาก Gameplay state เท่านั้น (Inventory/Upgrade/Pause/Trade ใช้ state machine — ยังไม่ใช่ modal)
    if (_stateMachine.CurrentState != EGameState.Gameplay) return;
    // กัน menu โผล่ทับ modal อื่น (popup, phase transition ฯลฯ ที่อยู่ใน stack)
    if (_modalStack.HasAny) return;

    Open();
  }

  public void Open()
  {
    if (_isOpen) return;

    BuildVisibleEntries();

    if (_visibleEntries.Count == 0)
    {
#if UNITY_EDITOR
      Debug.LogWarning("[HintMenuController] No unlocked entries — menu has nothing to show");
#endif
      return;
    }

    _isOpen = true;
    _modalStack.Push(this);  // stack จัดการ timeScale + block input

    _pageIndex = 0;

    _view.ShowMenu();
    _view.ShowPage(_visibleEntries[_pageIndex], _pageIndex, _visibleEntries.Count);
  }

  public void Close()
  {
    if (!_isOpen) return;

    _isOpen = false;
    _modalStack.Pop(this);  // stack restore timeScale + unblock input
    _view.Hide();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _view.OnToggleRequested -= HandleToggle;
    _view.OnCloseRequested -= HandleClose;
    _view.OnPrevPageRequested -= HandlePrevPage;
    _view.OnNextPageRequested -= HandleNextPage;
    _disposed = true;

    if (_isOpen)
      _modalStack.Pop(this);
  }

  // ==========================
  // Internal
  // ==========================

  private void BuildVisibleEntries()
  {
    _visibleEntries.Clear();
    foreach (var entry in _library.Entries)
    {
      if (entry.UnlockedByDefault || _state.IsUnlocked(entry.Id))
        _visibleEntries.Add(entry);
    }
  }

  private void HandleToggle() => Toggle();

  private void HandleClose() => Close();

  private void HandlePrevPage()
  {
    if (_pageIndex <= 0) return;
    _pageIndex--;
    _view.ShowPage(_visibleEntries[_pageIndex], _pageIndex, _visibleEntries.Count);
  }

  private void HandleNextPage()
  {
    if (_pageIndex >= _visibleEntries.Count - 1) return;
    _pageIndex++;
    _view.ShowPage(_visibleEntries[_pageIndex], _pageIndex, _visibleEntries.Count);
  }
}
