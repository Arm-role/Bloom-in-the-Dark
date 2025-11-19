using System.Collections.Generic;
using UnityEngine;

public class PlantHarvestHandler : MonoBehaviour
{
    [SerializeField] private PlantLootData plantItem;

    public ItemStack[] GetHarvestLoot(ToolItem toolUsed)
    {
        var results = new List<ItemStack>();

        foreach (var drop in plantItem.drops)
        {
            int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

            if (toolUsed != null && toolUsed.HasBonus)
            {
                if (Random.value < drop.bonusChance)
                    amount++;
            }

            results.Add(new ItemStack(drop.item, amount));
        }

        return results.ToArray();
    }

    public ItemStack[] GetHarvestLoot()
    {
        var results = new List<ItemStack>();

        foreach (var drop in plantItem.drops)
        {
            int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
            results.Add(new ItemStack(drop.item, amount));
        }

        return results.ToArray();
    }
}
