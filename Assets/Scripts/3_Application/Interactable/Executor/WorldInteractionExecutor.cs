#nullable enable

using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public sealed class WorldInteractionExecutor
{
  private readonly SpawnerHandle _spawner;
  private readonly PlayerProgression _playerProgression;
  private readonly ItemFactory _itemFactory;
  private readonly WorldTileManager _tileManager;
  private readonly PlayerInventory _playerInventory;
  private readonly IAudioService? _audio;
  private readonly IInteractionSoundConfig? _soundConfig;

  public Action<PlayerExpResult>? OnExpResult;

  public WorldInteractionExecutor(
      SpawnerHandle spawner,
      PlayerProgression playerProgression,
      ItemFactory itemFactory,
      WorldTileManager tileManager,
      PlayerInventory playerInventory,
      IAudioService? audio = null,
      IInteractionSoundConfig? soundConfig = null)
  {
    _spawner = spawner;
    _playerProgression = playerProgression;
    _itemFactory = itemFactory;
    _tileManager = tileManager;
    _playerInventory = playerInventory;
    _audio = audio;
    _soundConfig = soundConfig;
  }

  public async Task<bool> Execute(WorldAction action, WorldCell worldCell)
  {
    if (action == null) return false;
    if (worldCell == null) return false;

    var cellPos = worldCell.WorldCenter;

    if (action.AddTile != null && action.TileTargetLayer != ETileLayerType.None)
    {
      _tileManager.TryAddTile(worldCell.CellPos, action.TileTargetLayer, action.AddTile);
      PlayAt(_soundConfig?.OnTilePlace, cellPos);
    }

    if (action.RemoveTile && action.TileTargetLayer != ETileLayerType.None)
    {
      _tileManager.TryRemoveTile(worldCell.CellPos, action.TileTargetLayer);
      PlayAt(_soundConfig?.OnTileRemove, cellPos);
    }


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
      if (objectDestroyed)
        PlayAt(_soundConfig?.OnDestructibleBreak, cellPos);
    }

    // --------------------
    // Give Rewards (CONDITIONED) — exp and loot share the same gate so a
    // clearable grants neither until it is actually destroyed.
    // --------------------

    bool grantRewards =
        action.RewardCondition == ERewardCondition.Immediate ||
        (action.RewardCondition == ERewardCondition.OnObjectDestroyed && objectDestroyed);

    if (grantRewards)
    {
      if (action.Exp > 0)
        _playerProgression.AddExp(action.Exp);

      if (action.ItemRewards.Count > 0)
      {
        GiveRewards(action.ItemRewards);
        PlayAt(_soundConfig?.OnPickup, cellPos);
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
        PlayAt(_soundConfig?.OnPickup, action.SourcePosition);
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
#if UNITY_EDITOR
          Debug.LogError("Infinite reward loop detected");
#endif
          break;
        }

        var newInstance = _itemFactory.Create(stack.ItemData);
        remaining = _playerInventory.AddItem(newInstance, remaining);
      }
    }
  }

  private void PlayAt(SoundKey? key, Vector3 worldPos)
  {
    if (_audio == null || key == null) return;
    _audio.PlaySFX(key, worldPos);
  }

  public void RemoveObject(GameObject obj) => _tileManager.RemoveObject(obj);
}

public readonly struct PlayerExpResult
{
  public readonly int Exp;
  public readonly Vector3 Hitbox;

  public PlayerExpResult(
      int exp,
      Vector3 hitbox)
  {
    Exp = exp;
    Hitbox = hitbox;
  }
}
