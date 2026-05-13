using UnityEngine;

public class ItemFactory
{
  private readonly IItemDefinitionProvider _itemProvider;
  private readonly IStatDatabase _statDatabase;
  private readonly IUpgradeContainer _upgradeContainer;

  public ItemFactory(
      IItemDefinitionProvider itemProvider,
      IStatDatabase statDatabase,
      IUpgradeContainer upgradeContainer)
  {
    _itemProvider = itemProvider;
    _statDatabase = statDatabase;
    _upgradeContainer = upgradeContainer;
  }

  public IItemInstance Create(int itemId)
  {
    var def = _itemProvider.GetItem(itemId);

    if (def == null)
      return null;

    return new ItemInstanceBase(def, _statDatabase, _upgradeContainer);
  }

  public IItemInstance Create(IItemDefinition data)
  {
    Debug.Log(data.Name);
    return new ItemInstanceBase(data, _statDatabase, _upgradeContainer);
  }
}