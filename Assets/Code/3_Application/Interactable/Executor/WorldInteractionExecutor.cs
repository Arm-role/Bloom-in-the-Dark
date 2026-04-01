using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class WorldInteractionExecutor
{
  private readonly SpawnerHandle _spawner;
  private readonly PlayerProgression _playerProgression;
  private readonly ItemFactory _itemFactory;
  private readonly WorldTileManager _tileManager;
  private readonly PlayerInventory _playerInventory;

  public Action<PlayerExpResult> OnExpResult;

  public WorldInteractionExecutor(
      SpawnerHandle spawner,
      PlayerProgression playerProgression,
      ItemFactory itemFactory,
      WorldTileManager tileManager,
      PlayerInventory playerInventory)
  {
    _spawner = spawner;
    _playerProgression = playerProgression;
    _itemFactory = itemFactory;
    _tileManager = tileManager;
    _playerInventory = playerInventory;
  }

  public async Task<bool> Execute(WorldAction action, WorldCell worldCell)
  {
    if (action == null) return false;

    if (action.AddTile != null && action.TileTargetLayer != ETileLayerType.None)
      _tileManager.TryAddTile(worldCell.CellPos, action.TileTargetLayer, action.AddTile);

    if (action.RemoveTile && action.TileTargetLayer != ETileLayerType.None)
      _tileManager.TryRemoveTile(worldCell.CellPos, action.TileTargetLayer);


    bool objectDestroyed = false;

    // --------------------
    // Apply Damage
    // --------------------
    if (action.DamageTarget > 0f &&
        worldCell.Object != null)
    {
      var destructible =
          worldCell.Object.GetComponent<ClearableState>();

      if (destructible != null)
      {
        Debug.Log("DamageTarget : " + action.DamageTarget);
        objectDestroyed =
            destructible.ApplyDamage(action.DamageTarget);
      }
    }

    if (action.PlaceObjectId > 0)
    {
      GameObject ob = await _spawner.SpawnAsync(action.PlaceObjectId, worldCell.WorldCenter);
      _tileManager.TryPlaceObject(ob);
    }

    if (objectDestroyed || action.RemoveObject)
    {
      _tileManager.RemoveObject(worldCell.Object);
    }

    // --------------------
    // Give Rewards (CONDITIONED)
    // --------------------

    if (action.Exp > 0)
    {
      _playerProgression.AddExp(action.Exp);
    }

    if (action.ItemRewards.Count > 0)
    {
      if (action.RewardCondition == ERewardCondition.Immediate ||
          (action.RewardCondition == ERewardCondition.OnObjectDestroyed &&
           objectDestroyed))
      {
        GiveRewards(action.ItemRewards);
      }
    }

    return true;
  }

  public async Task<bool> Execute(WorldAction action)
  {
    if (action == null) return false;

    if (action.Exp > 0)
    {
      _playerProgression.AddExp(action.Exp);

      var finalExp = Mathf.RoundToInt(action.Exp);
      OnExpResult?.Invoke(new PlayerExpResult(finalExp, action.SourcePosition));
    }

    if (action.ItemRewards.Count > 0)
    {
      if (action.RewardCondition == ERewardCondition.Immediate ||
          (action.RewardCondition == ERewardCondition.OnObjectDestroyed))
      {
        GiveRewards(action.ItemRewards);
      }
    }

    return true;
  }

  private void GiveRewards(List<ItemStack> rewards)
  {
    foreach (var stack in rewards)
    {
      if (stack == null || stack.ItemData == null)
        continue;

      IItemInstance instance =
          stack.Instance ?? _itemFactory.Create(stack.ItemData);

      int remaining =
          _playerInventory.AddItem(instance, stack.Count);

      var guard = 0;

      while (remaining > 0)
      {
        guard++;

        if (guard > 100)
        {
          Debug.LogError("Infinite reward loop detected");
          break;
        }

        var newInstance = _itemFactory.Create(stack.ItemData);
        remaining = _playerInventory.AddItem(newInstance, remaining);
      }
    }
  }
}

public readonly struct PlayerExpResult
{
  public readonly int Energy;
  public readonly Vector3 Hitbox;

  public PlayerExpResult(
      int energy,
      Vector3 hitbox)
  {
    Energy = energy;
    Hitbox = hitbox;
  }
}