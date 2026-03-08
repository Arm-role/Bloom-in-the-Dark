using System.Collections.Generic;

public static class LootTableFactory
{
  public static ILootTable Create(LootTableData data)
  {
    var drops = new List<LootDrop>();

    foreach (var d in data.drops)
    {
      drops.Add(new LootDrop(
        d.item,
        d.minAmount,
        d.maxAmount,
        d.bonusChance));
    }

    return new LootTable(drops, new UnityLootRandom());
  }
}