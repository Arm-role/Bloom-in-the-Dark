#nullable enable

using System;

// Interface สำหรับ TurnSystem (MonoBehaviour ใน 5_Views) เพื่อให้ 3_Application ใช้ได้
//
// Events (จังหวะต่างกัน — เลือกตามต้องการ):
//   OnNextTurn               — ยิงที่ midpoint ของ transition canvas (state เปลี่ยนแล้ว, animation ยังเล่นอยู่)
//                              ใช้กับ system ที่ต้อง react ทันที (plant growth, phase music)
//   OnTurnTransitionComplete — ยิงหลัง transition canvas เล่นจบ (player ควบคุมได้แล้ว)
//                              ใช้กับ UI ที่ pause game ทับ (hint popup) เพื่อกันชน transition
public interface ITurnSystem
{
  event Action<ETurnState> OnNextTurn;
  event Action<ETurnState> OnTurnTransitionComplete;

  ETurnState Current { get; }

  // Day counter — เพิ่มทีละ 1 ตอนเข้า ETurnState.Farm. Endless mode เริ่มที่ EndlessStartDay (default 51)
  // ใช้สำหรับ schedule logic (WanderingTrader, event triggers ที่อิงวัน)
  int CurrentDay { get; }
}
