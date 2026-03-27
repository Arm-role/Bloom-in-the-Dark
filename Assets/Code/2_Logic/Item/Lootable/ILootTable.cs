public interface ILootTable
{
  (int Exp, ItemStack[]) RollLoot(IItemDefinition toolUsed = null);
}