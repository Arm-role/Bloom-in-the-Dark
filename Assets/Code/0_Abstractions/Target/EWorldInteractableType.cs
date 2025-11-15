using System;

[Flags]
public enum EWorldInteractableType
{
    None = 0,

    Harvestable = 1 << 0,     // สามารถเก็บเกี่ยวได้ เช่น พืชโตเต็มที่
    Mineable = 1 << 1,        // ใช้ pickaxe ขุดได้ เช่น หิน
    Tillable = 1 << 2,        // ใช้ hoe พรวนดินได้
    Waterable = 1 << 3,       // รดน้ำได้ เช่น ดินหรือพืช
    Talkable = 1 << 4,        // สนทนาได้ เช่น NPC
    Attackable = 1 << 5,      // โจมตีได้ เช่น ศัตรู
    UsableStation = 1 << 6,   // ใช้งานสถานี เช่น เตา, โต๊ะคราฟต์
    Lootable = 1 << 7,        // เปิดกล่อง / เก็บของได้
    Collectible = 1 << 8,     // ของตกพื้น เก็บเข้ากระเป๋าได้
    BuildableArea = 1 << 9,   // พื้นที่ที่สามารถสร้างสิ่งปลูกสร้างได้

    All = ~0
}