using System.Threading.Tasks;
using UnityEngine;

public class PlantableAction : ITileAction
{
    private SpawnerHandle _spawner;

    public PlantableAction(SpawnerHandle spawner)
    {
        _spawner = spawner;
    }

    public bool CanInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        return ctx.ItemInstance.Data is SeedItem seed
               && !state.HasPlacedObject;
    }

    public async Task<bool> TryInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        Debug.Log("Planting seed");

        var itemData = ctx.ItemInstance.Data;

        if (itemData is SeedItem seed)
        {
            Debug.Log("Seed");
            var plant = await _spawner.SpawnAsync(seed.PlantName, state.WorldCenter);

            if (plant == null) return false;

            state.PlacedObject = plant;
            return true;
        }

        return false;
    }
}
