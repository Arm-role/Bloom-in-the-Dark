public class LootDrop
{
  public IItemDefinition Item { get; }
  public int MinAmount { get; }
  public int MaxAmount { get; }
  public float BonusChance { get; }

  public LootDrop(IItemDefinition item, int minAmount, int maxAmount, float bonusChance)
  {
    Item = item;
    MinAmount = minAmount;
    MaxAmount = maxAmount;
    BonusChance = bonusChance;
  }
}