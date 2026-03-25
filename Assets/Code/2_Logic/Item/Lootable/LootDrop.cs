public class LootDrop
{
  public IItemDefinition Item { get; }
  public int MinAmount { get; }
  public int MaxAmount { get; }
  public float BonusChance { get; }
  public int MinExp { get; internal set; }
  public int MaxExp { get; internal set; }

  public LootDrop(IItemDefinition item, int minAmount, int maxAmount, float bonusChance)
  {
    Item = item;
    MinAmount = minAmount;
    MaxAmount = maxAmount;
    BonusChance = bonusChance;
  }
}