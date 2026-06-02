#nullable enable

using System;

// Book-style menu (page navigation, no tabs):
//   - Page content (กลาง): 1 page = 1 IHintEntry (title + sprite + description)
//   - Footer: prev/next buttons + page indicator "X / N"
//
// Controller จัดการ pageIndex; view รับผิดชอบ render และ raise event
// Toggle key (H) อยู่ใน IPlayerInput.OnHintToggle — controller subscribe เอง ไม่ผ่าน view
public interface IHintMenuView
{
  // Close / Esc / Space
  event Action OnCloseRequested;

  // คลิก ←/→ → controller เลื่อน pageIndex
  event Action OnPrevPageRequested;
  event Action OnNextPageRequested;

  // เปิด menu ครั้งแรก — view เริ่ม visible (page content ตามมาทันทีจาก ShowPage)
  void ShowMenu();

  // อัปเดต page content — title + sprite + description + page indicator "X / N"
  // view จัดการ enable/disable prev/next buttons ตาม bounds เอง
  void ShowPage(IHintEntry entry, int pageIndex, int pageCount);

  void Hide();
}
