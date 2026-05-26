#nullable enable

// ผลของ PlayerInventory.Place — ใช้บอก InventoryService ว่าเล่นเสียงไหน
public enum PlaceResult
{
  RestoredToSameSlot,   // วางกลับช่องเดิม
  PlacedOnEmpty,        // target ว่าง → drop
  Merged,               // same-Data → AddAmount เข้า stack เดิม (อาจมี overflow คืน source)
  Swapped,              // diff-Data → swap target ↔ source
  ReturnedToSource      // same-Data + target เต็ม → คืน source ไม่ swap
}
