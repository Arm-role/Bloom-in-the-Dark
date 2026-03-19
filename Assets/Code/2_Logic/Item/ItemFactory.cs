public class ItemFactory
{
  private readonly IItemDefinitionProvider _itemProvider;
  private readonly IUpgradeContainer _upgradeContainer;

  public ItemFactory(
      IItemDefinitionProvider itemProvider,
      IUpgradeContainer upgradeContainer)
  {
    _itemProvider = itemProvider;
    _upgradeContainer = upgradeContainer;
  }

  public IItemInstance Create(int itemId)
  {
    var def = _itemProvider.GetItem(itemId);

    if (def == null)
      return null;

    return new ItemInstanceBase(def, _upgradeContainer);
  }

  public IItemInstance Create(IItemDefinition data)
  {
    return new ItemInstanceBase(data, _upgradeContainer);
  }
}