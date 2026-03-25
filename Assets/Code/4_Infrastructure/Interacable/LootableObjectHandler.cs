using UnityEngine;

public class LootableObjectHandler : MonoBehaviour, ILootableHandler
{
  [SerializeField] private LootTableData lootTableData;

  private ILootTable _lootTable;
  private void Awake()
  {
    _lootTable = LootTableFactory.Create(lootTableData);
  }

  public (int Exp, ItemStack[]) GetHarvestLoot(IItemDefinition toolUsed)
  {
    return _lootTable.RollLoot(toolUsed);
  }

  public (int Exp, ItemStack[]) GetHarvestLoot()
  {
    return _lootTable.RollLoot();
  }
}