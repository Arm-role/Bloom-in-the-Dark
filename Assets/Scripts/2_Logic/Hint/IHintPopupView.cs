#nullable enable

using System;

// Modal popup UI contract — concrete impl คือ HintPopupView MonoBehaviour ใน 5_Views
// View ทำหน้าที่ render เท่านั้น (Title/Body/Media); logic การ pause + mark viewed อยู่ที่ controller
public interface IHintPopupView
{
  // ผู้เล่นกดปิด (Close button / click outside / Esc) → controller resume game + hide
  event Action OnCloseRequested;

  // Display entry — media null → ซ่อนภาพ, แสดงข้อความอย่างเดียว
  void Show(IHintEntry entry);

  void Hide();
}
