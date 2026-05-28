#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Book-style orchestrator (flat page list — no tabs):
//   - Open → filter unlocked entries → pause game → ShowMenu → ShowPage 0
//   - Prev/Next → clamp pageIndex → ShowPage
//   - Close → resume game → Hide
//
// บทบาทคู่:
//   HintPopupController = forced reading (welcome / AutoShowOnUnlock)
//   HintMenuController  = browse mode (book + page navigation)
public sealed class HintMenuController : IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintMenuView _view;

  // Flat list — ตามลำดับใน Library (UnlockedByDefault + unlocked entries)
  private readonly List<IHintEntry> _visibleEntries = new();

  private bool _isOpen;
  private float _prevTimeScale = 1f;
  private int _pageIndex;
  private bool _disposed;

  public HintMenuController(
    IHintLibrary library,
    IHintState state,
    IHintMenuView view)
  {
    _library = library;
    _state = state;
    _view = view;

    _view.OnToggleRequested += HandleToggle;
    _view.OnCloseRequested += HandleClose;
    _view.OnPrevPageRequested += HandlePrevPage;
    _view.OnNextPageRequested += HandleNextPage;
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

    BuildVisibleEntries();

    if (_visibleEntries.Count == 0)
    {
#if UNITY_EDITOR
      Debug.LogWarning("[HintMenuController] No unlocked entries — menu has nothing to show");
#endif
      return;
    }

    _isOpen = true;
    _prevTimeScale = Time.timeScale;
    Time.timeScale = 0f;

    _pageIndex = 0;

    _view.ShowMenu();
    _view.ShowPage(_visibleEntries[_pageIndex], _pageIndex, _visibleEntries.Count);
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
    _view.OnPrevPageRequested -= HandlePrevPage;
    _view.OnNextPageRequested -= HandleNextPage;
    _disposed = true;

    if (_isOpen)
      Time.timeScale = _prevTimeScale;
  }

  // ==========================
  // Internal — state machine
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
