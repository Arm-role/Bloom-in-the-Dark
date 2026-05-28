#nullable enable

using System.Collections.Generic;

// Collection contract — concrete impl คือ HintLibrary SO ใน 4_Infrastructure
// Designer ลาก HintEntry ลง trigger slot — Entries รวม slot ทั้งหมดอัตโนมัติสำหรับ menu
public interface IHintLibrary
{
  IReadOnlyList<IHintEntry> Entries { get; }

  // คืน null ถ้าไม่เจอ — caller log warning เอง
  IHintEntry? GetById(string id);

  // ==========================
  // Event trigger slots
  // ==========================
  // designer drag HintEntry ลง field ใน Library inspector
  // null = ไม่มี trigger สำหรับ event นั้น (HintUnlockBinder ข้าม)

  // Forced popup ตอน gameplayState.Enter ครั้งแรก
  IHintEntry? WelcomeEntry { get; }

  // PlayerController.OnDamaged ครั้งแรก (player ถูก hit)
  IHintEntry? DamageEntry { get; }
}
