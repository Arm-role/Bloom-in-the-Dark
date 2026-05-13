public class ItemInstanceBase : IItemInstance
{
  public IItemDefinition Data { get; }
  public int Level { get; private set; }
  public ItemStatService Stats { get; private set; }

  private readonly IUpgradeContainer _upgradeContainer;

  public ItemInstanceBase(
    IItemDefinition data,
    IStatDatabase statDatabase,
    IUpgradeContainer upgradeContainer,
    int level = 1)
  {
    Data = data;
    Level = level;
    _upgradeContainer = upgradeContainer;

    Stats = new(data, statDatabase, upgradeContainer);
  }

  public void AddLevel(int amount = 1)
  {
    Level += amount;
  }
}