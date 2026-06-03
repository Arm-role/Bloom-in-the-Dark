using UnityEngine;

// MonoBehaviour ที่แปะบน world object (enemy, plant, building) — รับ harvest call
// → RollLoot ผ่าน LootTable
//
// Census injection: installer (หรือ spawn pipeline สำหรับ runtime-spawned object)
// ต้องเรียก Initialize(census) หลัง Awake. ถ้าไม่ inject → _census==null → LootContext.None
// → GlobalCap filter ไม่ทำงาน (graceful fallback — drop ปกติ)
public class LootableObjectHandler : MonoBehaviour, ILootableHandler
{
  [SerializeField] private LootTableData lootTableData;

  private ILootTable _lootTable;
  private IItemCensus _census;

  private void Awake()
  {
    _lootTable = LootTableFactory.Create(lootTableData);
  }

  // เรียกจาก installer (pre-existing) หรือ spawn pipeline (runtime-spawned)
  // หลัง Awake. Idempotent — เรียกซ้ำ override ตัวเก่าได้
  public void Initialize(IItemCensus census)
  {
    _census = census;
  }

  public (int Exp, ItemStack[]) GetHarvestLoot(IItemDefinition toolUsed)
  {
    return _lootTable.RollLoot(BuildContext(), toolUsed);
  }

  public (int Exp, ItemStack[]) GetHarvestLoot()
  {
    return _lootTable.RollLoot(BuildContext());
  }

  private LootContext BuildContext()
    => _census != null ? new LootContext(_census) : LootContext.None;
}
