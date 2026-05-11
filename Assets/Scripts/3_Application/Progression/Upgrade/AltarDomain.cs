using System;
using System.Collections.Generic;

public class AltarDomain
{
  private EAltarMode _mode = EAltarMode.None;
  private IItemDefinition _buffItem;
  private readonly Dictionary<int, int> _craftContainer = new();

  private readonly GameTag _moonBloomTag;
  private readonly GameTag _plantTag;
  private readonly RequestDatabase _requestDatabase;

  public event Action<IItemDefinition, bool> OnUpgradeReady;
  public event Action<List<UpgradeRequestDefinition>> OnCraftProgressChanged;
  public event Action<UpgradeRequestDefinition> OnCraftReady;
  public event Action OnCleared;

  public AltarDomain(RequestDatabase requestDatabase, GameTag moonBloomTag, GameTag plantTag)
  {
    _requestDatabase = requestDatabase;
    _moonBloomTag = moonBloomTag;
    _plantTag = plantTag;
  }

  public void PlaceItem(IItemDefinition item)
  {
    switch (_mode)
    {
      case EAltarMode.None:        HandleFirstItem(item);   break;
      case EAltarMode.UpgradeBuff: HandleBuffSecond(item);  break;
      case EAltarMode.Craft:       HandleCraft(item);       break;
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
      FireUpgrade(item, false);
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
      FireUpgrade(item, true);
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
    _craftContainer.Clear();
    OnCleared?.Invoke();
  }
}
