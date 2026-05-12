using System;
using System.Collections.Generic;
using UnityEngine;

public class AltarDomain
{
  private EAltarMode _mode = EAltarMode.None;
  private IItemDefinition _buffItem;
  private IItemDefinition _lockedPlant;
  private int _placedCount;
  private UpgradeRequestDefinition _pendingCraft;

  private readonly GameTag _moonBloomTag;
  private readonly GameTag _plantTag;
  private readonly RequestDatabase _requestDatabase;
  private readonly Func<int> _getRequiredCount;
  private readonly Dictionary<int, int> _craftContainer = new();

  public event Action<int, int, IItemDefinition> OnUpgradeProgressChanged; // current, required, plant
  public event Action<IItemDefinition, bool> OnUpgradeReady;
  public event Action<List<UpgradeRequestDefinition>> OnCraftProgressChanged;
  public event Action<UpgradeRequestDefinition> OnCraftPreviewReady;
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

  // Returns false if the item was rejected (wrong type for current mode)
  public bool PlaceItem(IItemDefinition item)
  {
    if (_pendingCraft != null) return false;

    return _mode switch
    {
      EAltarMode.None        => HandleFirstItem(item),
      EAltarMode.UpgradeBuff => HandleBuffSecond(item),
      EAltarMode.Upgrade     => HandleUpgradePlant(item),
      EAltarMode.Craft       => HandleCraft(item),
      _                      => false,
    };
  }

  public void RemoveItem(IItemDefinition item)
  {
    switch (_mode)
    {
      case EAltarMode.Upgrade:
      case EAltarMode.UpgradeBuff:
        RemoveFromUpgradeMode(item);
        break;
      case EAltarMode.Craft:
        RemoveFromCraftMode(item);
        break;
    }
  }

  public void ConfirmCraft()
  {
    if (_pendingCraft == null) return;
    var confirmed = _pendingCraft;
    _pendingCraft = null;
    OnCraftReady?.Invoke(confirmed);
    Clear();
  }

  // =============================
  // Place Handlers
  // =============================

  private bool HandleFirstItem(IItemDefinition item)
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
    return true;
  }

  private bool HandleBuffSecond(IItemDefinition item)
  {
    if (!item.HasTag(_plantTag)) return false;
    AddPlant(item);
    return true;
  }

  private bool HandleUpgradePlant(IItemDefinition item)
  {
    if (!item.HasTag(_plantTag)) return false;
    AddPlant(item);
    return true;
  }

  private bool HandleCraft(IItemDefinition item)
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
        _pendingCraft = request;
        OnCraftPreviewReady?.Invoke(request);
        return true;
      }
    }
    return true;
  }

  // =============================
  // Remove Handlers
  // =============================

  private void RemoveFromUpgradeMode(IItemDefinition item)
  {
    if (item.HasTag(_moonBloomTag))
    {
      _buffItem = null;
      if (_placedCount == 0)
        Clear();
      else
        _mode = EAltarMode.Upgrade; // downgrade — plants remain without buff
      return;
    }

    if (_lockedPlant == null || item.ID != _lockedPlant.ID) return;

    _placedCount = Mathf.Max(0, _placedCount - 1);

    if (_placedCount == 0)
    {
      _lockedPlant = null;
      if (_buffItem == null)
        Clear(); // nothing left
      // else: moonbloom still placed, stay in UpgradeBuff waiting for plant
      return;
    }

    OnUpgradeProgressChanged?.Invoke(_placedCount, _getRequiredCount(), _lockedPlant);
  }

  private void RemoveFromCraftMode(IItemDefinition item)
  {
    if (!_craftContainer.ContainsKey(item.ID)) return;

    _craftContainer[item.ID]--;
    if (_craftContainer[item.ID] <= 0)
      _craftContainer.Remove(item.ID);

    _pendingCraft = null;

    if (_craftContainer.Count == 0)
    {
      Clear();
      return;
    }

    var itemIds = new List<int>(_craftContainer.Keys);
    if (UpgradeRequestQuery.TryGetRequestsUsingItem(itemIds, _requestDatabase.requests, out var matching))
      OnCraftProgressChanged?.Invoke(matching);
    else
      OnCraftProgressChanged?.Invoke(new List<UpgradeRequestDefinition>());
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
    _pendingCraft = null;
    _craftContainer.Clear();
    OnCleared?.Invoke();
  }
}
