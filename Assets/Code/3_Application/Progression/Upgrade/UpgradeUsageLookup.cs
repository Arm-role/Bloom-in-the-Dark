using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeUsageLookup : MonoBehaviour
{
  private Dictionary<int, int> itemContainer = new();
  private RequestDatabase _requestDatabase;
  private IUpgradeRequestView _viewRequest;
  private IUpgradeManagerView _viewManager;

  public Action<UpgradeRequestDefinition> OnRequestMatched;

  private void Start()
  {
    OnRequestMatched += RequestMatched;
  }

  public void Initialize(
    RequestDatabase requestDatabase,
    IUpgradeRequestView requestView,
    IUpgradeManagerView managerView)
  {
    _requestDatabase = requestDatabase;
    _viewRequest = requestView;
    _viewManager = managerView;
  }

  private void RequestMatched(UpgradeRequestDefinition request)
  {
    itemContainer.Clear();
    _viewRequest.Hide();
    _viewManager.OnOpenUpgradePopup(request.GameKeyId);
  }

  private void ShowView(List<UpgradeRequestDefinition> requests)
  {
    Debug.Log("ShowView " + requests.Count);
    var bars = new List<RequestBarViewModel>();

    foreach (var request in requests)
    {
      var bar = new RequestBarViewModel
      {
        upgradeName = request.UpgradeName,
        slotViewModels = new List<RequestSlotViewModel>()
      };

      foreach (var ingredient in request.Ingredients)
      {
        bar.slotViewModels.Add(new RequestSlotViewModel
        {
          ItemId = ingredient.item.RuntimeTag.Hash,
          Amount = ingredient.amount
        });
      }

      bars.Add(bar);
    }

    _viewRequest.SetSlots(bars);
  }

  public void GetItemInstacne(int itemId)
  {
    if (!itemContainer.ContainsKey(itemId))
      itemContainer[itemId] = 0;

    itemContainer[itemId]++;

    var itemIds = itemContainer.Keys.ToList();

    UpgradeRequestQuery.TryGetRequestsUsingItem(
           itemIds,
           _requestDatabase.requests,
           out var requests);

    ShowView(requests);
    CheckRequests();
  }

  private void CheckRequests()
  {
    foreach (var request in _requestDatabase.requests)
      if (IsMatch(request))
        OnRequestMatched?.Invoke(request);
  }

  private bool IsMatch(UpgradeRequestDefinition request)
  {
    foreach (var ingredient in request.Ingredients)
    {
      if (!itemContainer.TryGetValue(ingredient.item.RuntimeTag.Hash, out var count))
        return false;

      if (count < ingredient.amount)
        return false;
    }

    return true;
  }
}