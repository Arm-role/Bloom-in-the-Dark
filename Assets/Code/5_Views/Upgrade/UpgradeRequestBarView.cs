using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeRequestBarView : MonoBehaviour
{
  [SerializeField] private TMP_Text upgradeName;
  [SerializeField] private UpgradeRequestSlotView slotPrefab;
  [SerializeField] private Transform contentParent;
  [SerializeField] private Button button;

  private readonly List<UpgradeRequestSlotView> _slotViews = new();

  private IItemIconProvider _iconDatabase;

  private RequestBarViewModel _model;
  public event Action<RequestBarViewModel> SelectUpgradeRequest;

  // =============================
  // Initialization
  // =============================

  public void Initialize(
    IItemIconProvider itemIconDatabase)
  {
    _iconDatabase = itemIconDatabase;

    button.onClick.RemoveAllListeners();
    button.onClick.AddListener(OnClick);
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


  public void Bind(RequestBarViewModel model)
  {
    _model = model;
  }

  private void OnClick()
  {
    SelectUpgradeRequest?.Invoke(_model);
  }

  // =============================
  // Icon
  // =============================

  private Sprite ResolveIcon(int itemId)
  {
    return _iconDatabase.GetIcon(itemId);
  }
}
