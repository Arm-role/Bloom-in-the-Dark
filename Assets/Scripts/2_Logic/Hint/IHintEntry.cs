#nullable enable

using UnityEngine;

// Data contract สำหรับ 1 hint — concrete impl คือ HintEntry SO ใน 4_Infrastructure
// ใช้ string Id เพื่อความเรียบง่าย (ไม่ต้องสร้าง SO key ต่อ hint) — designer ตั้งให้ unique
public interface IHintEntry
{
  string Id { get; }                // unique key (e.g., "intro_pickup")
  string Title { get; }             // ชื่อใน menu + popup header
  string Description { get; }       // body text (รองรับ TMP rich tags)
  Sprite? Media { get; }            // null = text-only popup
  HintCategory Category { get; }
  bool UnlockedByDefault { get; }   // true → โผล่ใน menu ตั้งแต่เกมเริ่ม
}
