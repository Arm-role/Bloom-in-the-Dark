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
  // Event trigger slots (HintEntry refs)
  // ==========================
  // designer drag HintEntry ลง field ใน Library inspector
  // null = ไม่มี trigger สำหรับ event นั้น (HintUnlockBinder ข้าม)

  // Forced popup ตอน gameplayState.Enter ครั้งแรก
  IHintEntry? WelcomeEntry { get; }

  // PlayerHealth.OnChanged cross HealthLowThreshold ลงมาครั้งแรก (HP ตกถึงเกณฑ์)
  IHintEntry? DamageEntry { get; }

  // PlayerEnergy ≤ EnergyLowThreshold ครั้งแรก (D2)
  IHintEntry? EnergyLowEntry { get; }

  // ITurnSystem.OnNextTurn → ETurnState.Preparation ครั้งแรก (D2)
  IHintEntry? PhasePrepareEntry { get; }

  // ITurnSystem.OnNextTurn → ETurnState.Battle ครั้งแรก (D2)
  IHintEntry? PhaseBattleEntry { get; }

  // ItemInteractionAction.OnActionCommitted ครั้งแรก (player ใช้ item ทำ action สำเร็จ)
  IHintEntry? ActionHintEntry { get; }

  // PlayerInventory.OnItemAdded → counter พืช upgradable ครบ PlantCountThreshold (D4)
  IHintEntry? PlantCountEntry { get; }

  // PlayerInventory.OnItemAdded → inventory มีทั้ง branch + stone (D4)
  IHintEntry? BranchStoneComboEntry { get; }

  // ==========================
  // Detection slots (tag refs)
  // ==========================

  // Tag สำหรับ category-based detection
  ItemTag? UpgradablePlantTag { get; }
  ItemTag? BranchTag { get; }
  ItemTag? StoneTag { get; }

  // ==========================
  // Config fields
  // ==========================

  float EnergyLowThreshold { get; }
  float HealthLowThreshold { get; }
  int PlantCountThreshold { get; }
}
