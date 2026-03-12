public static class ItemFactory
{
  public static IItemInstance Create(IItemDefinition data)
  {
    var item = new ItemInstanceBase(data, GlobalUpgradeSystem.Instance);
    return item;
  }
}


