#nullable enable

// ทะเบียน item ที่ player ครอบครองอยู่ทั่วเกม — ใช้สำหรับ loot cap filter
// V1 sources: PlayerInventory (Hotbar + MainInventory) + OfferingAltarController slots
//
// Concrete impl: ItemCensus (3_Application/Inventory/ItemCensus.cs)
public interface IItemCensus
{
  int CountAcrossWorld(IItemDefinition item);
}
