public interface ILootTable
{
  ItemStack[] RollLoot(IItemDefinition toolUsed = null);
}