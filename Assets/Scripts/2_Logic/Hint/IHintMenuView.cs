#nullable enable

using System;
using System.Collections.Generic;

// Menu UI contract — concrete impl คือ HintMenuView MonoBehaviour ใน 5_Views
// View handle input (กด H toggle, Esc close) + raise event → controller จัดการ flow
public interface IHintMenuView
{
  // Player กดปุ่ม toggle (ค่า default = H) — ทำงานทั้งตอน menu เปิดและปิด → controller decide
  event Action OnToggleRequested;

  // Player กด Close / Esc / outside click ขณะ menu โผล่
  event Action OnCloseRequested;

  // Player คลิก entry tile → controller delegate ต่อให้ HintPopupController.Show(id)
  event Action<string> OnEntryClicked;

  // entries เป็น list ที่ filter แล้ว (unlocked + UnlockedByDefault) ตามจัดโดย controller
  void ShowMenu(IReadOnlyList<IHintEntry> entries);

  void Hide();
}
