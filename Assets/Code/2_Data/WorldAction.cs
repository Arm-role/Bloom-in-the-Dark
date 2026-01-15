using System.Collections.Generic;

public class WorldAction
{
    public string PlaceObject;
    public bool RemoveObject;

    public IBaseTileData AddTile;
    public ETileLayerType TileTargetLayer;
    public bool RemoveTile;

    public List<ItemStack> ItemRewards = new();

    public int DamageTarget;
}