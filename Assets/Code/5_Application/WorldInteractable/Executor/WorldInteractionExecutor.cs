using UnityEngine;
using System.Threading.Tasks;

public class WorldInteractionExecutor
{
    private ItemLibrary _library;
    private SpawnerHandle _spawner;
    private PlayerInventory _playerInventory;

    public WorldInteractionExecutor(
       ItemLibrary library,
       SpawnerHandle spawner,
       PlayerInventory playerInventory)
    {
        _library = library;
        _spawner = spawner;
        _playerInventory = playerInventory;
    }

    public async Task<bool> Execute(WorldAction action, GameObject target)
    {
        if (action == null) return false;

        if (action.SpawnObject != null)
            await _spawner.SpawnAsync(action.SpawnObject, target.transform.position);

        if (action.ItemRewards.Count > 0)
        {
            foreach (var stack in action.ItemRewards)
            {
                if (stack == null || stack.ItemData == null) continue;

                IItemInstance instanceToAdd = stack.Instance ?? CreateItemInstance(stack.ItemData);

                int remaining = _playerInventory.AddItem(instanceToAdd, stack.Count);

                while (remaining > 0)
                {
                    var newInstance = CreateItemInstance(stack.ItemData);
                    remaining = _playerInventory.AddItem(newInstance, remaining);
                }
            }
        }

        if (action.DestroySelf)
        {
            if (target.TryGetComponent<IDestructible>(out var destructible))
            {
                destructible.RequestDestruction();
            }
        }

        return true;
    }

    private IItemInstance CreateItemInstance(IItemData item)
    {
        switch (item.Type)
        {
            case EItemType.Plant: return new PlantItemInstance(item);
            case EItemType.Tool: return new ToolItemInstance(item);
            case EItemType.Seed: return new SeedItemInstance(item);
            case EItemType.Building: return new BuildingItemInstance(item);
            default: return null;
        }
    }
}
