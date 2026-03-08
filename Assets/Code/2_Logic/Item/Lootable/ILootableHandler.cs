public interface ILootableHandler
{
  public ItemStack[] GetHarvestLoot(IToolItemData toolUsed);
  public ItemStack[] GetHarvestLoot();
}