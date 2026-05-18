public class ExpDrop
{
  public int MinExp { get; }
  public int MaxExp { get; }
  public float BonusChance { get; }

  public ExpDrop(int minExp, int maxExp, float bonusChance)
  {
    MinExp = minExp;
    MaxExp = maxExp;
    BonusChance = bonusChance;
  }
}