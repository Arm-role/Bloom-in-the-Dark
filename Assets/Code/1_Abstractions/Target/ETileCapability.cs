using System;

[Flags]
public enum ETileCapability
{
    None = 0, // เดินได้ ไม่มี interaction

    // ---------- Movement ----------
    BlocksMovement = 1 << 0, // เดินไม่ได้ (ผนัง, หินก้อนใหญ่, ตึก)
    Water = 1 << 1, // พื้นน้ำ (โดยปกติเดินไม่ได้ เว้นแต่มีระบบเรือ/ทักษะ)

    // ---------- World Actions ----------
    CanHarvest = 1 << 2, // เก็บเกี่ยวได้ (พืชโตเต็มที่)
    CanMine = 1 << 3, // ขุดได้ (rock)
    CanTill = 1 << 4, // พรวนดินได้ (Hoe)
    CanWater = 1 << 5, // รดน้ำได้ (watering can)
    CanPlant = 1 << 6, // ใส่อันนี้เพื่อรองรับการปลูกพืชบนดินไถ

    // ---------- Interaction ----------
    Interactable = 1 << 7, // ใช้งาน (โต๊ะคราฟต์, เตา, station)
    Lootable = 1 << 8, // เปิดเก็บของได้ (chest)
    Collectible = 1 << 9, // เก็บเข้ากระเป๋าได้ (item dropped)

    HasNPC = 1 << 10, // คุยได้ + block การเดิน
    HasEnemy = 1 << 11, // โจมตีได้ + block การเดิน

    Buildable = 1 << 12, // ใช้เป็นพื้นที่วางสิ่งปลูกสร้าง
    
    All = ~0
}