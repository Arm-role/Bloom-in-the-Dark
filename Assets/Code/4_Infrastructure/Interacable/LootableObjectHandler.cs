using UnityEngine;

public class LootableObjectHandler : MonoBehaviour, ILootableHandler
{
  [SerializeField] private LootTableData lootTableData;

  private ILootTable _lootTable;
  private void Awake()
  {
    _lootTable = LootTableFactory.Create(lootTableData);
  }

  public ItemStack[] GetHarvestLoot(IToolItemData toolUsed)
  {
    return _lootTable.RollLoot(toolUsed);
  }

  public ItemStack[] GetHarvestLoot()
  {
    return _lootTable.RollLoot();
  }
}