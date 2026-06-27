#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

// Orchestrator: lookup entry → push modal stack → show view → mark viewed
// Multi-popup queue: Show ที่มาขณะ popup เปิด → enqueue (dedup ทั้งกับ current + ใน queue)
//                   Close → pop next จาก queue ถ้ามี (stack ยังคง push), queue ว่าง → pop stack
//
// Modal stack จัดการ timeScale + input block ให้แทน — controller ไม่ต้อง track _prevState/_prevTimeScale เอง
// PausesGame=true → stack.OnFirstPausingPush set timeScale=0; OnLastPausingPop restore 1
public sealed class HintPopupController : IModalUI, IDisposable
{
  private readonly IHintLibrary _library;
  private readonly IHintState _state;
  private readonly IHintPopupView _view;
  private readonly ModalUIStack _modalStack;

  private readonly Queue<string> _pendingIds = new();
  private readonly HashSet<string> _pendingSet = new();  // O(1) dup check คู่กับ Queue

  private bool _isOpen;
  private string? _currentId;
  private bool _disposed;

  public HintPopupController(
    IHintLibrary library,
    IHintState state,
    IHintPopupView view,
    ModalUIStack modalStack)
  {
    _library = library;
    _state = state;
    _view = view;
    _modalStack = modalStack;

    _view.OnCloseRequested += HandleCloseRequested;
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

    // เปิดครั้งแรก (closed → open) — push stack (stack จัดการ timeScale + block input)
    _modalStack.Push(this);
    DisplayEntry(entry);
  }

  public void Close()
  {
    if (!_isOpen) return;

    // มี queue → แสดง entry ถัดไปทันที (stack ยัง push ตัวเองอยู่)
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

    // queue ว่าง → pop stack (stack restore timeScale + unblock input) + hide view
    _isOpen = false;
    _currentId = null;
    _modalStack.Pop(this);
    _view.Hide();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _view.OnCloseRequested -= HandleCloseRequested;
    _disposed = true;

    _pendingIds.Clear();
    _pendingSet.Clear();

    // Dispose ระหว่าง popup เปิด → pop stack (stack restore timeScale ผ่าน OnLastPausingPop)
    if (_isOpen)
      _modalStack.Pop(this);
  }

  // ==========================
  // Internal
  // ==========================

  // แสดง entry + mark state (เรียกได้ทั้งครั้งแรก + จาก queue) — stack push เรียบร้อยแล้ว
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
