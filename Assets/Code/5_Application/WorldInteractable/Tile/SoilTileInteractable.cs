using System.Threading.Tasks;
using UnityEngine;

public class SoilTileInteractable : TileInteractableAdapter
{
    private readonly TileBaseDataState _state;
    private readonly TilemapService _tilemapService;
    private readonly SpawnerHandle _spawner;

    public SoilTileInteractable(
        TileBaseDataState state,
        TilemapService tilemapService,
        SpawnerHandle spawner)
        : base(state, priority: 1f)
    {
        _state = state;
        _tilemapService = tilemapService;
        _spawner = spawner;
    }

    public override bool CanInteract(InteractionHandleContext ctx)
    {
        if (ctx.ItemInstance.Data is SeedItem) return true;
        if (ctx.ItemInstance.Data.Name == "Pickaxe") return true;

        return false;
    }

    public async override Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        var v = CanInteract(ctx);
        if (!v) return false;

        var itemData = ctx.ItemInstance.Data;

        if (itemData.Name == "Pickaxe")
        {
            if (_state.IsOccupied)
            {
                var destructible = _state.PlacedObject.GetComponent<IDestructible>();
                destructible.RequestDestruction();
            }
            else
            {
                _tilemapService.RemoveTile(_state.WorldCenter, ETileLayerType.Interactable);
            }
        }
        else if (itemData is SeedItem seed)
        {
            var plant = await _spawner.SpawnAsync(seed.PlantName, _state.WorldCenter);

            if (plant == null) return false;

            _state.PlacedObject = plant;
        }
        return true;
    }
}
