using UnityEngine;

public class RequestShowView : MonoBehaviour
{
  [SerializeField] private UpgradeRequestStoneView[] upgradeRequests;
  private IItemIconProvider _iconDatabase;

  public void Initialize(
  IItemIconProvider itemIconDatabase)
  {
    _iconDatabase = itemIconDatabase;
  }

  // =============================
  // Rendering
  // =============================

  public void GetItemRequestWorld(ItemKey item)
  {
    //for (int i = 0; i < slots.Count; i++)
    //{
    //  var slotView = Instantiate(slotPrefab, contentParent);
    //  _slotViews.Add(slotView);
    //}

    //for (int i = 0; i < _slotViews.Count; i++)
    //{
    //  var model = slots[i];

    //  upgradeRequests[i].SetAmount(model.Amount);
    //  upgradeRequests[i].SetIcon(ResolveIcon(model.ItemId));
    //}
  }

  private Sprite ResolveIcon(int itemId)
  {
    return _iconDatabase.GetIcon(itemId);
  }
}