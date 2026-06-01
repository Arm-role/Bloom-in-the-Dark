#nullable enable

// Contract สำหรับ UI ที่เป็น modal (popup, menu, dialog) ที่ register เข้า ModalUIStack
// Modal UI ต้อง implement interface นี้เพื่อ:
//   - รับ Esc dismiss routing จาก stack (top เท่านั้นที่รับ)
//   - บอก stack ว่าตัวเองต้อง pause game (timeScale=0) ตอนเปิดหรือไม่
//
// PausesGame=true  → Hint, Pause, Upgrade, PhaseTransition (player ห้ามทำอะไร)
// PausesGame=false → Inventory, Trade (player จัด UI แต่โลกยังเดิน)
//
// ทั้งสองกรณี: DragDropController + ItemInteractionAction ถูก block ตอน stack ไม่ว่าง (กัน click leak)
public interface IModalUI
{
  bool IsOpen { get; }

  // true → stack จะตั้ง Time.timeScale=0 ตอน push (ถ้าเป็น pausing modal ตัวแรก)
  // false → stack ไม่แตะ timeScale (โลกเดินต่อระหว่าง modal เปิด)
  bool PausesGame { get; }

  // Stack route Esc มาที่ top modal — strict consume (ไม่ส่งต่อ)
  void HandleDismiss();
}
