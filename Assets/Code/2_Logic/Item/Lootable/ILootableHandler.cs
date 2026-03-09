public interface ILootableHandler
{
  public ItemStack[] GetHarvestLoot(IItemDefinition toolUsed);
  public ItemStack[] GetHarvestLoot();
}