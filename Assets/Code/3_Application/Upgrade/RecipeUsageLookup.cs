using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeUsageLookup : MonoBehaviour
{
  [SerializeField] private RecipeDatabase recipeDatabase;
  [SerializeField] private ItemKey[] keys;
  [SerializeField] private ItemKey key1;
  [SerializeField] private ItemKey key2;
  [SerializeField] private ItemKey key3;

  private Dictionary<ItemKey, List<RecipeDefinition>> _usageMap = new();
  private Dictionary<ItemKey, int> itemContainer = new();
  private IUpgradeRequestView _view;

  public Action<UpgradeRequestDefinition> OnRequestMatched;

  private void Start()
  {
    BuildRecipe(recipeDatabase.recipes);


    OnRequestMatched += request =>
    {
      UpgradeManager.Instance.OpenUpgradePopup();
      itemContainer.Clear();
      _view.Hide();
      Debug.Log("RequestComplete " + request.upgradeName);
    };
  }
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.I))
    {
      GetItemInstacne(key1.RuntimeTag.Hash);
    }
    if (Input.GetKeyDown(KeyCode.O))
    {
      GetItemInstacne(key2.RuntimeTag.Hash);
    }
    if (Input.GetKeyDown(KeyCode.P))
    {
      GetItemInstacne(key3.RuntimeTag.Hash);
    }
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

  public void BuildRecipe(IEnumerable<RecipeDefinition> recipes)
  {
    foreach (var recipe in recipes)
    {
      foreach (var ingredient in recipe.ingredients)
      {
        if (!_usageMap.TryGetValue(ingredient.item, out var list))
        {
          list = new List<RecipeDefinition>();
          _usageMap.Add(ingredient.item, list);
        }

        list.Add(recipe);
      }
    }
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

    // ป้องกัน key ไม่มี
    if (!itemContainer.ContainsKey(item))
      itemContainer[item] = 0;

    itemContainer[item]++;

    var items = itemContainer.Keys.ToList();
    Debug.Log(item.Id);

    UpgradeRequestQuery.TryGetRequestsUsingItem(
           items,
           recipeDatabase.requests,
           out var requests);

    ShowView(requests);
    CheckRequests();
  }

  public IEnumerable<RecipeDefinition> GetRecipesUsing(ItemKey item)
  {
    if (_usageMap.TryGetValue(item, out var list))
      return list;

    return Array.Empty<RecipeDefinition>();
  }

  private void CheckRequests()
  {
    foreach (var request in recipeDatabase.requests)
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