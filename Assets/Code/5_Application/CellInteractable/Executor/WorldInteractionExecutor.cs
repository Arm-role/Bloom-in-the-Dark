using UnityEngine;
using System.Threading.Tasks;

public class WorldInteractionExecutor
{
    private readonly SpawnerHandle _spawner;
    private readonly WorldTileManager _tileManager;
    private PlayerInventory _playerInventory;

    public WorldInteractionExecutor(
        SpawnerHandle spawner,
        WorldTileManager tileManager,
        PlayerInventory playerInventory)
    {
        _spawner = spawner;
        _tileManager = tileManager;
        _playerInventory = playerInventory;
    }

    public async Task<bool> Execute(WorldAction action, WorldCell worldCell)
    {
        if (action == null) return false;

        if (action.PlaceObject != null)
        {
            GameObject ob = await _spawner.SpawnAsync(action.PlaceObject, worldCell.WorldCenter);
            _tileManager.TryPlaceObject(worldCell.CellPos, ob);
        }

        if (action.RemoveObject)
            _tileManager.RemoveObject(worldCell.CellPos);

        if (action.AddTile != null && action.TileTargetLayer != ETileLayerType.None)
            _tileManager.TryAddTile(worldCell.CellPos, action.TileTargetLayer, action.AddTile);

        if (action.RemoveTile && action.TileTargetLayer != ETileLayerType.None)
            _tileManager.TryRemoveTile(worldCell.CellPos, action.TileTargetLayer);

        if (action.ItemRewards.Count > 0)
        {
            foreach (var stack in action.ItemRewards)
            {
                if (stack == null || stack.ItemData == null) continue;

                IItemInstance instanceToAdd = stack.Instance ?? ItemFactory.Create(stack.ItemData);

                int remaining = _playerInventory.AddItem(instanceToAdd, stack.Count);

                while (remaining > 0)
                {
                    var newInstance = ItemFactory.Create(stack.ItemData);
                    remaining = _playerInventory.AddItem(newInstance, remaining);
                }
            }
        }
        return true;
    }
}