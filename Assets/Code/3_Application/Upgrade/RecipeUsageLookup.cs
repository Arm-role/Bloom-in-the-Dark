using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeUsageLookup : MonoBehaviour
{
  [SerializeField] private RequestDatabase requestDatabase;

  [SerializeField] private ItemKey[] keys;
  [SerializeField] private ItemKey key1;
  [SerializeField] private ItemKey key2;
  [SerializeField] private ItemKey key3;

  private Dictionary<ItemKey, int> itemContainer = new();
  private IUpgradeRequestView _view;

  public Action<UpgradeRequestDefinition> OnRequestMatched;

  private void Start()
  {
    OnRequestMatched += request =>
    {
      UpgradeManager.Instance.OpenUpgradePopup();
      itemContainer.Clear();
      _view.Hide();
      Debug.Log("RequestComplete " + request.upgradeName);
    };
  }

  public void Initialize(IUpgradeRequestView requestView)
  {
    _view = requestView;
  }

  private void ShowView(List<UpgradeRequestDefinition> requests)
  {
    var bars = new List<RequestBarViewModel>();

    foreach (var request in requests)
    {
      var bar = new RequestBarViewModel
      {
        upgradeName = request.upgradeName,
        slotViewModels = new List<RequestSlotViewModel>()
      };

      foreach (var ingredient in request.ingredients)
      {
        bar.slotViewModels.Add(new RequestSlotViewModel
        {
          ItemId = ingredient.item.RuntimeTag.Hash,
          Amount = ingredient.amount
        });
      }

      bars.Add(bar);
    }

    _view.SetSlots(bars);
  }

  public void GetItemInstacne(int itemId)
  {
    ItemKey item = null;
    for (int i = 0; i < keys.Length; i++)
    {
      if (keys[i].RuntimeTag.Hash == itemId)
      {
        item = keys[i];
        break;
      }
    }

    if (item == null)
      return;

    if (!itemContainer.ContainsKey(item))
      itemContainer[item] = 0;

    itemContainer[item]++;

    var items = itemContainer.Keys.ToList();
    Debug.Log(item.Id);

    UpgradeRequestQuery.TryGetRequestsUsingItem(
           items,
           requestDatabase.requests,
           out var requests);

    ShowView(requests);
    CheckRequests();
  }

  private void CheckRequests()
  {
    foreach (var request in requestDatabase.requests)
    {
      if (IsMatch(request))
      {
        OnRequestMatched?.Invoke(request);
      }
    }
  }

  private bool IsMatch(UpgradeRequestDefinition request)
  {
    foreach (var ingredient in request.ingredients)
    {
        if (!itemContainer.TryGetValue(ingredient.item, out var count))
            return false;

        if (count < ingredient.amount)
            return false;
    }

    return true;
  }
}