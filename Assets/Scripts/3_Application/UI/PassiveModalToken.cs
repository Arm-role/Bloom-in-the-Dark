#nullable enable

// Token-based IModalUI — caller ควบคุม Begin/End เอง
// ใช้กับ modal flow ที่ไม่มี dedicated controller class (เช่น Upgrade ที่ trigger ผ่าน UpgradeListener event,
// GameApplication เป็นคน wire push/pop)
//
// HandleDismiss = no-op — token เปิด/ปิดผ่าน Begin/End เท่านั้น (player กด Esc ไม่ปิด)
//   ถ้าต้องการ user-dismissible modal ที่มี logic เปิด/ปิด → implement IModalUI ตรง ไม่ใช้ token
public sealed class PassiveModalToken : IModalUI
{
  private readonly ModalUIStack _stack;
  private readonly bool _pausesGame;
  private bool _isOpen;

  public PassiveModalToken(ModalUIStack stack, bool pausesGame)
  {
    _stack = stack;
    _pausesGame = pausesGame;
  }

  public bool IsOpen => _isOpen;
  public bool PausesGame => _pausesGame;
  public void HandleDismiss() { /* no-op */ }

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
