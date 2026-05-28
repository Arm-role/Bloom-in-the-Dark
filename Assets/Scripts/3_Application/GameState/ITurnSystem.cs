#nullable enable

using System;

// Interface สำหรับ TurnSystem (MonoBehaviour ใน 5_Views) เพื่อให้ 3_Application ใช้ได้
// HintUnlockBinder (D2) subscribe OnNextTurn เพื่อ trigger hint ตอน Preparation/Battle ครั้งแรก
public interface ITurnSystem
{
  event Action<ETurnState> OnNextTurn;

  ETurnState Current { get; }
}
