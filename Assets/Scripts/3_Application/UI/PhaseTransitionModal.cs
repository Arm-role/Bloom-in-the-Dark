#nullable enable

// Modal สำหรับ phase transition canvas — block input ระหว่างที่ TurnView เล่น fade + label animation
//
// PausesGame=false เพราะ TurnView ใช้ Time.deltaTime + WaitForSeconds (scaled) ใน TransitionRoutine
//   ถ้า timeScale=0 → coroutine แช่ → onComplete callback ไม่ยิง → ค้างถาวร
// HandleDismiss=no-op — ห้าม player กด Esc skip transition (กิน event เงียบๆ ไม่ส่งต่อ)
//
// ใช้: TurnSystem.NextTurn เรียก Begin() ก่อน PlayTurnTransition, End() ใน onComplete
public sealed class PhaseTransitionModal : IModalUI
{
  private readonly ModalUIStack _stack;
  private bool _isOpen;

  public PhaseTransitionModal(ModalUIStack stack)
  {
    _stack = stack;
  }

  public bool IsOpen => _isOpen;
  public bool PausesGame => false;
  public void HandleDismiss() { /* no-op — player ห้าม skip transition */ }

  public void Begin()
  {
    if (_isOpen) return;
    _isOpen = true;
    _stack.Push(this);
  }

  public void End()
  {
    if (!_isOpen) return;
    _isOpen = false;
    _stack.Pop(this);
  }
}
