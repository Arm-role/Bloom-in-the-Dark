public class LootDrop
{
  public IItemDefinition Item { get; }
  public int MinAmount { get; }
  public int MaxAmount { get; }
  public float BonusChance { get; }

  // Cap: ถ้า player ครอบครอง item นี้รวมทั่วเกม (Inventory + Altar) >= cap → skip drop
  // ถ้า count + roll amount > cap → clamp ลงให้พอดี cap
  // 0 (default) = ไม่ filter (drop ปกติ)
  public int GlobalCap { get; }

  public int MinExp { get; internal set; }
  public int MaxExp { get; internal set; }

  public LootDrop(IItemDefinition item, int minAmount, int maxAmount, float bonusChance, int globalCap = 0)
  {
    Item = item;
    MinAmount = minAmount;
    MaxAmount = maxAmount;
    BonusChance = bonusChance;
    GlobalCap = globalCap;
  }
}
