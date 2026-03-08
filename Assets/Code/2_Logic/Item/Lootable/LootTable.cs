using System.Collections.Generic;

public class LootTable : ILootTable
{
  private readonly List<LootDrop> _drops;
  private readonly ILootRandom _random;

  public LootTable(List<LootDrop> drops, ILootRandom random)
  {
    _drops = drops;
    _random = random;
  }

  public ItemStack[] RollLoot(IToolItemData toolUsed = null)
  {
    var results = new List<ItemStack>();

    foreach (var drop in _drops)
    {
      int amount = _random.Range(drop.MinAmount, drop.MaxAmount + 1);

      if (toolUsed != null && toolUsed.HasBonus)
      {
        if (_random.Value() < drop.BonusChance)
          amount++;
      }

      results.Add(new ItemStack(drop.Item, amount));
    }

    return results.ToArray();
  }
}