#nullable enable

public sealed class ItemFactory
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

  // Return null เมื่อ id ไม่มีใน provider — caller (เช่น UpgradeManagerPresenter) ใช้ null เป็น "ไม่มี item version"
  public IItemInstance? Create(int itemId)
  {
    var def = _itemProvider.GetItem(itemId);
    if (def == null) return null;

    return new ItemInstanceBase(def, _statDatabase, _upgradeContainer);
  }

  public IItemInstance Create(IItemDefinition data)
    => new ItemInstanceBase(data, _statDatabase, _upgradeContainer);
}
