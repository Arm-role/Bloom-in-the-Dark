public class LootDrop
{
  public IItemDefinition Item { get; }

  // 0.0-1.0 — chance ที่ entry นี้จะ drop เลย (gate ก่อน amount roll)
  // 1.0 (default) = drop ทุกครั้ง (เทียบเท่ากับ behavior เดิม)
  // 0.3 = 30% drop, 70% skip
  public float DropChance { get; }

  // เมื่อผ่าน chance gate แล้ว → roll amount ใน [Min, Max]
  public int MinAmount { get; }
  public int MaxAmount { get; }

  // tool ที่มี tag "Item.BonusChance" → roll bonus เพิ่ม +1 amount ด้วย chance นี้
  public float BonusChance { get; }

  // Cap: ถ้า player ครอบครอง item นี้รวมทั่วเกม (Inventory + Altar) >= cap → skip drop
  // ถ้า count + roll amount > cap → clamp ลงให้พอดี cap
  // 0 (default) = ไม่ filter (drop ปกติ)
  public int GlobalCap { get; }

  public int MinExp { get; internal set; }
  public int MaxExp { get; internal set; }

  public LootDrop(
    IItemDefinition item,
    int minAmount,
    int maxAmount,
    float bonusChance,
    int globalCap = 0,
    float dropChance = 1.0f)
  {
    Item = item;
    MinAmount = minAmount;
    MaxAmount = maxAmount;
    BonusChance = bonusChance;
    GlobalCap = globalCap;
    DropChance = dropChance;
  }
}
