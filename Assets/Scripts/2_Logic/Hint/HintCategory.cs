#nullable enable

// Category สำหรับ filter ใน Hint Menu — designer set ใน HintEntry SO
// เพิ่ม value ใหม่ได้ตามต้องการ (อย่าลบ value เก่าโดยไม่ migrate enum index)
public enum HintCategory
{
  General,
  Combat,
  Farming,
  Inventory,
  Interaction,
  World,
}
