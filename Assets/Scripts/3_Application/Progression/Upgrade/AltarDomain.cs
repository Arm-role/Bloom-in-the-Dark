using System;
using System.Collections.Generic;
using UnityEngine;

public class AltarDomain
{
  private EAltarMode _mode = EAltarMode.None;
  private IItemDefinition _buffItem;
  private IItemDefinition _lockedPlant;
  private int _placedCount;

  private readonly GameTag _moonBloomTag;
  private readonly GameTag _plantTag;
  private readonly RequestDatabase _requestDatabase;
  private readonly Func<int> _getRequiredCount;
  private readonly Dictionary<int, int> _craftContainer = new();

  public event Action<int, int, IItemDefinition> OnUpgradeProgressChanged; // current, required, plant
  public event Action<IItemDefinition, bool> OnUpgradeReady;
  public event Action<List<UpgradeRequestDefinition>> OnCraftProgressChanged;
  public event Action<UpgradeRequestDefinition> OnCraftReady;
  public event Action OnCleared;

  public AltarDomain(
    RequestDatabase requestDatabase,
    GameTag moonBloomTag,
    GameTag plantTag,
    Func<int> getRequiredCount)
  {
    _requestDatabase = requestDatabase;
    _moonBloomTag = moonBloomTag;
    _plantTag = plantTag;
    _getRequiredCount = getRequiredCount;
  }

  public void PlaceItem(IItemDefinition item)
  {
    switch (_mode)
    {
      case EAltarMode.None:        HandleFirstItem(item);  break;
      case EAltarMode.UpgradeBuff: HandleBuffSecond(item); break;
      case EAltarMode.Upgrade:     HandleUpgradePlant(item); break;
      case EAltarMode.Craft:       HandleCraft(item);      break;
    }
  }

  // =============================
  // Mode Handlers
  // =============================

  private void HandleFirstItem(IItemDefinition item)
  {
    if (item.HasTag(_moonBloomTag))
    {
      _mode = EAltarMode.UpgradeBuff;
      _buffItem = item;
    }
    else if (item.HasTag(_plantTag))
    {
      _mode = EAltarMode.Upgrade;
      AddPlant(item);
    }
    else
    {
      _mode = EAltarMode.Craft;
      HandleCraft(item);
    }
  }

  private void HandleBuffSecond(IItemDefinition item)
  {
    if (item.HasTag(_plantTag))
    {
      _mode = EAltarMode.UpgradeBuff;
      AddPlant(item);
    }
  }

  private void HandleUpgradePlant(IItemDefinition item)
  {
    if (item.HasTag(_plantTag))
      AddPlant(item);
  }

  private void HandleCraft(IItemDefinition item)
  {
    if (!_craftContainer.ContainsKey(item.ID))
      _craftContainer[item.ID] = 0;
    _craftContainer[item.ID]++;

    var itemIds = new List<int>(_craftContainer.Keys);
    if (UpgradeRequestQuery.TryGetRequestsUsingItem(itemIds, _requestDatabase.requests, out var matching))
      OnCraftProgressChanged?.Invoke(matching);

    foreach (var request in _requestDatabase.requests)
    {
      if (IsCraftMatch(request))
      {
        OnCraftReady?.Invoke(request);
        Clear();
        return;
      }
    }
  }

  // =============================
  // Upgrade Count
  // =============================

  private void AddPlant(IItemDefinition plant)
  {
    if (_lockedPlant == null)
      _lockedPlant = plant;

    _placedCount++;
    int required = _getRequiredCount();

    OnUpgradeProgressChanged?.Invoke(_placedCount, required, _lockedPlant);

    if (_placedCount >= required)
      FireUpgrade(_lockedPlant, _mode == EAltarMode.UpgradeBuff);
  }

  // =============================
  // Helpers
  // =============================

  private void FireUpgrade(IItemDefinition plant, bool buffed)
  {
    OnUpgradeReady?.Invoke(plant, buffed);
    Clear();
  }

  private bool IsCraftMatch(UpgradeRequestDefinition request)
  {
    foreach (var ingredient in request.Ingredients)
    {
      if (!_craftContainer.TryGetValue(ingredient.item.RuntimeTag.Hash, out var count))
        return false;
      if (count < ingredient.amount)
        return false;
    }
    return true;
  }

  private void Clear()
  {
    _mode = EAltarMode.None;
    _buffItem = null;
    _lockedPlant = null;
    _placedCount = 0;
    _craftContainer.Clear();
    OnCleared?.Invoke();
  }
}