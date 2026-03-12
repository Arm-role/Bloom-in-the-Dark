using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeRequestBarView : MonoBehaviour 
{
  [SerializeField] private TMP_Text upgradeName;
  [SerializeField] private UpgradeRequestSlotView slotPrefab;
  [SerializeField] private Transform contentParent;

  private readonly List<UpgradeRequestSlotView> _slotViews = new();

  private IItemIconProvider _iconDatabase;

  // =============================
  // Initialization
  // =============================

  public void Initialize(
    IItemIconProvider itemIconDatabase)
  {
    _iconDatabase = itemIconDatabase;
  }

  // =============================
  // Rendering
  // =============================

  public void SetName(string name)
  {
    upgradeName.text = name;
  }

  public void SetSlots(IReadOnlyList<RequestSlotViewModel> slots)
  {
    for (int i = 0; i < slots.Count; i++)
    {
      var slotView = Instantiate(slotPrefab, contentParent);
      _slotViews.Add(slotView);
    }

    for (int i = 0; i < _slotViews.Count; i++)
    {
      var model = slots[i];

      _slotViews[i].SetAmount(model.Amount);
      _slotViews[i].SetIcon(ResolveIcon(model.ItemId));
    }
  }

  // =============================
  // Cooldown
  // =============================

  private Sprite ResolveIcon(int itemId)
  {
    return _iconDatabase.GetIcon(itemId);
  }
}
