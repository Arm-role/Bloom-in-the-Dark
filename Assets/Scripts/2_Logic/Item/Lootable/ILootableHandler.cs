public interface ILootableHandler
{
  (int Exp, ItemStack[]) GetHarvestLoot(IItemDefinition toolUsed);
  (int Exp, ItemStack[]) GetHarvestLoot();
}