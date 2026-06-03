#nullable enable

// Context ที่ส่งให้ LootTable.RollLoot — รวมข้อมูลรอบนอกที่ filter ต้องใช้
// V1: แค่ IItemCensus (cap check). ถ้าวันหลังเพิ่ม filter อื่น (วัน, weather) → เพิ่ม field ใน struct นี้
//
// LootContext.None = filter ปิด (Census==null) — สำหรับ test หรือ legacy call site
public readonly struct LootContext
{
  public readonly IItemCensus? Census;

  public LootContext(IItemCensus? census)
  {
    Census = census;
  }

  public static LootContext None => new(null);
}
