#nullable enable

using System;
using System.Collections.Generic;

// Central registry ของ modal UI ที่เปิดอยู่ — LIFO order (top = ตัวล่าสุดที่ push)
//
// Track 2 ระดับแยกกัน:
//   - HasAny (any modal)           → DragDropController + ItemInteractionAction block input
//   - HasPausingModal (PausesGame) → Time.timeScale=0
// เพราะ Inventory/Trade เปิดอยู่ = block input แต่ไม่ pause (โลกเดินต่อ)
//
// Events:
//   OnFirstPush         → push ตัวแรก (0 → 1)         : block input, cancel pending action
//   OnStackEmpty        → pop ตัวสุดท้าย (1 → 0)      : re-enable input
//   OnFirstPausingPush  → pausing modal ตัวแรก enter  : set timeScale=0
//   OnLastPausingPop    → pausing modal ตัวสุดท้าย leave: restore timeScale=1
//
// Dismiss: Router (subscribe IPlayerInput.OnDismiss) เรียก RouteDismiss() → top.HandleDismiss()
public sealed class ModalUIStack
{
  private readonly List<IModalUI> _stack = new();
  private int _pausingCount;

  public bool HasAny => _stack.Count > 0;
  public bool HasPausingModal => _pausingCount > 0;
  public int Count => _stack.Count;
  public IModalUI? Top => _stack.Count > 0 ? _stack[_stack.Count - 1] : null;

  public event Action? OnFirstPush;
  public event Action? OnStackEmpty;
  public event Action? OnFirstPausingPush;
  public event Action? OnLastPausingPop;

  // Push modal — dedup ป้องกัน register ซ้ำ
  public void Push(IModalUI modal)
  {
    if (modal == null) return;
    if (_stack.Contains(modal)) return;

    bool wasEmpty = _stack.Count == 0;
    bool wasNonPausing = _pausingCount == 0;

    _stack.Add(modal);
    if (modal.PausesGame) _pausingCount++;

    if (wasEmpty) OnFirstPush?.Invoke();
    if (modal.PausesGame && wasNonPausing) OnFirstPausingPush?.Invoke();
  }

  // Pop modal — ลบจาก list (ไม่จำเป็นต้องเป็น top เสมอ; ถ้า A เปิด B เปิดทับ A ปิด — ออกได้)
  public void Pop(IModalUI modal)
  {
    if (modal == null) return;
    if (!_stack.Remove(modal)) return;

    if (modal.PausesGame)
    {
      _pausingCount--;
      if (_pausingCount == 0) OnLastPausingPop?.Invoke();
    }

    if (_stack.Count == 0) OnStackEmpty?.Invoke();
  }

  // Esc routed มาที่ top — strict consume (ไม่ส่งต่อแม้ top จะไม่ทำอะไร)
  public void RouteDismiss() => Top?.HandleDismiss();
}
