using System;
using System.Collections.Generic;

public class LootTable : ILootTable
{
  private readonly List<LootDrop> _drops;
  private readonly ExpDrop _expData;
  private readonly ILootRandom _random;

  public LootTable(List<LootDrop> drops, ExpDrop expData, ILootRandom random)
  {
    _drops = drops;
    _expData = expData;
    _random = random;
  }

  // Legacy overload — no cap filter (delegates to context-aware path with None)
  public (int Exp, ItemStack[]) RollLoot(IItemDefinition toolUsed = null)
    => RollLoot(LootContext.None, toolUsed);

  public (int Exp, ItemStack[]) RollLoot(LootContext context, IItemDefinition toolUsed = null)
  {
    var results = new List<ItemStack>();
    bool hasBonus = false;

    if (toolUsed != null)
      hasBonus = toolUsed.HasTag(TagLibrary.Get("Item.BonusChance"));

    foreach (var drop in _drops)
    {
      int amount = _random.Range(drop.MinAmount, drop.MaxAmount + 1);

      if (hasBonus)
      {
        if (_random.Value() < drop.BonusChance)
          amount++;
      }

      // GlobalCap filter — clamp to fit cap, skip drop if cap already reached
      // Census==null (LootContext.None) → ไม่ filter (ตามเดิม)
      if (drop.GlobalCap > 0 && context.Census != null)
      {
        int existing = context.Census.CountAcrossWorld(drop.Item);
        int remaining = drop.GlobalCap - existing;
        if (remaining <= 0) continue;             // เต็มแล้ว skip
        amount = Math.Min(amount, remaining);     // clamp ให้พอดี
      }

      if (amount <= 0) continue;
      results.Add(new ItemStack(drop.Item, amount));
    }

    int exp = _random.Range(_expData.MinExp, _expData.MaxExp + 1);

    if (hasBonus)
    {
      if (_random.Value() < _expData.BonusChance)
        exp++;
    }

    return (exp, results.ToArray());
  }
}
