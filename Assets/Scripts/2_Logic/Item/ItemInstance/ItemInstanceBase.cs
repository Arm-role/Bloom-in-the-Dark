#nullable enable

// "Base" suffix — เปิดให้ subclass override Clone ได้ ห้าม seal
public class ItemInstanceBase : IItemInstance
{
  public IItemDefinition Data { get; }
  public int Level { get; private set; }
  public ItemStatService Stats { get; }

  private readonly IStatDatabase _statDatabase;
  private readonly IUpgradeContainer _upgradeContainer;

  public ItemInstanceBase(
    IItemDefinition data,
    IStatDatabase statDatabase,
    IUpgradeContainer upgradeContainer,
    int level = 1)
  {
    Data = data;
    Level = level;
    _statDatabase = statDatabase;
    _upgradeContainer = upgradeContainer;

    Stats = new ItemStatService(data, statDatabase, upgradeContainer);
  }

  public void AddLevel(int amount = 1)
  {
    Level += amount;
  }

  public virtual IItemInstance Clone()
    => new ItemInstanceBase(Data, _statDatabase, _upgradeContainer, Level);
}
