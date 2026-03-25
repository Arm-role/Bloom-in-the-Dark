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

  public (int Exp, ItemStack[]) RollLoot(IItemDefinition toolUsed = null)
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