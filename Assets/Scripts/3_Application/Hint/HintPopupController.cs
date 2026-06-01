#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Orchestrator: lookup entry → pause game → show view → mark viewed
// Multi-popup queue: Show ที่มาขณะ popup เปิด → enqueue (dedup ทั้งกับ current + ใน queue)
//                   Close → pop next จาก queue ถ้ามี (game ยัง pause), queue ว่าง → resume
// External caller (HintUnlockBinder TryUnlock + welcome Enter, menu click, debug) เรียก Show(id) จะ trigger flow ครบ
public sealed class HintPopupController : IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintPopupView _view;
  private readonly GameStateMachine _stateMachine;

  private readonly Queue<string> _pendingIds = new();
  private readonly HashSet<string> _pendingSet = new();  // O(1) dup check คู่กับ Queue

  private bool _isOpen;
  private string? _currentId;
  private float _prevTimeScale = 1f;
  private EGameState _prevState;
  private bool _disposed;

  public HintPopupController(
    IHintLibrary library,
    IHintState state,
    IHintPopupView view,
    GameStateMachine stateMachine)
  {
    _library = library;
    _state = state;
    _view = view;
    _stateMachine = stateMachine;

    _view.OnCloseRequested += HandleCloseRequested;
  }

  public bool IsOpen => _isOpen;

  // Open popup ของ entry id — ถ้า popup อื่นเปิดอยู่ → enqueue (แสดงต่อตอน close)
  // null/unknown id → log + ignore; duplicate (current หรือใน queue) → no-op
  public void Show(string id)
  {
#if UNITY_EDITOR
    Debug.Log($"[HintPopupController] Show called id='{id}' isOpen={_isOpen} pending={_pendingIds.Count}");
#endif
    var entry = _library.GetById(id);
    if (entry == null)
    {
#if UNITY_EDITOR
      Debug.LogWarning($"[HintPopupController] Hint id '{id}' not found in library");
#endif
      return;
    }

    if (_isOpen)
    {
      if (id == _currentId) return;
      if (!_pendingSet.Add(id)) return;
      _pendingIds.Enqueue(id);
#if UNITY_EDITOR
      Debug.Log($"[HintPopupController] Queued id='{id}' (queue size={_pendingIds.Count})");
#endif
      return;
    }

    // เปิดครั้งแรก (closed → open) — save state + pause game + switch to Hint state (block input)
    _prevState = _stateMachine.CurrentState;
    _prevTimeScale = Time.timeScale;
    Time.timeScale = 0f;
    _stateMachine.ChangeState(EGameState.Hint);
    DisplayEntry(entry);
  }

  public void Close()
  {
    if (!_isOpen) return;

    // มี queue → แสดง entry ถัดไปทันที (ไม่ unpause)
    while (_pendingIds.Count > 0)
    {
      var nextId = _pendingIds.Dequeue();
      _pendingSet.Remove(nextId);

      var nextEntry = _library.GetById(nextId);
      if (nextEntry == null)
      {
#if UNITY_EDITOR
        Debug.LogWarning($"[HintPopupController] Queued hint id '{nextId}' missing — skip");
#endif
        continue;
      }

      DisplayEntry(nextEntry);
      return;
    }

    // queue ว่าง → resume + hide view + restore state machine
    _isOpen = false;
    _currentId = null;
    Time.timeScale = _prevTimeScale;
    _stateMachine.ChangeState(_prevState);
    _view.Hide();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _view.OnCloseRequested -= HandleCloseRequested;
    _disposed = true;

    _pendingIds.Clear();
    _pendingSet.Clear();

    // Dispose ระหว่าง popup เปิด → restore timeScale + state กัน game ค้าง pause/Hint
    if (_isOpen)
    {
      Time.timeScale = _prevTimeScale;
      _stateMachine.ChangeState(_prevState);
    }
  }

  // แสดง entry + mark state (เรียกได้ทั้งครั้งแรก + จาก queue) — assume game pause + _prevTimeScale ถูกตั้งแล้ว
  private void DisplayEntry(IHintEntry entry)
  {
    _isOpen = true;
    _currentId = entry.Id;

#if UNITY_EDITOR
    Debug.Log($"[HintPopupController] Showing entry title='{entry.Title}'");
#endif

    _view.Show(entry);
    _state.MarkViewed(entry.Id);
    // ดูแล้ว = unlock ด้วย → entry นี้จะโผล่ใน Menu ครั้งถัดไป (สำคัญสำหรับ welcome ที่ไม่ผ่าน TryUnlock)
    _state.Unlock(entry.Id);
  }

  private void HandleCloseRequested() => Close();
}
