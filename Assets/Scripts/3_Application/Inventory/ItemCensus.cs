#nullable enable

using System.Collections.Generic;

// Aggregator ของ item count จากทุก source ที่ player ครอบครอง:
//   - PlayerInventory (Hotbar + MainInventory ผ่าน CountItemAnywhere)
//   - OfferingAltarController slots (pre + post commit จนกว่าจะ Clear/RemoveItem)
//
// ใช้โดย LootTable.RollLoot สำหรับ GlobalCap filter (drop เฉพาะตอน count < cap, clamp ให้พอดี)
//
// Altar list สร้างที่ installer ตอน scene init (FindObjectsOfType<OfferingAltarController>
// หรือ collect จาก AltarController._offeringAltars) → static list, ไม่ dynamic
public sealed class ItemCensus : IItemCensus
{
  private readonly PlayerInventory _inventory;
  private readonly IReadOnlyList<OfferingAltarController> _altarSlots;

  public ItemCensus(PlayerInventory inventory, IReadOnlyList<OfferingAltarController> altarSlots)
  {
    _inventory = inventory;
    _altarSlots = altarSlots;
  }

  public int CountAcrossWorld(IItemDefinition item)
  {
    if (item == null) return 0;

    int sum = _inventory.CountItemAnywhere(item);

    foreach (var altar in _altarSlots)
    {
      if (altar == null) continue;  // destroyed mid-scene
      var held = altar.HeldItemDefinition;
      if (held != null && held.ID == item.ID) sum++;
    }

    return sum;
  }
}
