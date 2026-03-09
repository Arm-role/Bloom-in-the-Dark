public static class ItemFactory
{
  public static IItemInstance Create(IItemDefinition data)
  {
    return new ItemInstanceBase(data);
  }
}