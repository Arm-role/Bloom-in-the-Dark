using System;
using System.Collections.Generic;

public static class LootTableFactory
{
  public static ILootTable Create(LootTableData data)
  {
    var drops = new List<LootDrop>();

    var expData = new ExpDrop(
      data.expData.minAmount,
      data.expData.maxAmount,
      data.expData.bonusChance);

    foreach (var d in data.drops)
    {
      drops.Add(new LootDrop(
        d.item,
        d.minAmount,
        d.maxAmount,
        d.bonusChance));
    }

    return new LootTable(drops, expData, new UnityLootRandom());
  }
}