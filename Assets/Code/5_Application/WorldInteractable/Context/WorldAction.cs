using System.Collections.Generic;

public class WorldAction
{
    public string SpawnObject;
    public bool DestroySelf;

    public List<ItemStack> ItemRewards = new();

    public int DamageTarget;
}
