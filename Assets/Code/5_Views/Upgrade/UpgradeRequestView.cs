using System.Collections.Generic;
using UnityEngine;

public class UpgradeRequestView : MonoBehaviour, IUpgradeRequestView
{
  [SerializeField] private UpgradeRequestBarView barPrefab;
  [SerializeField] private Transform contentParent;

  private readonly List<UpgradeRequestBarView> _barViews = new();

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

  public void SetSlots(IReadOnlyList<RequestBarViewModel> bars)
  {
    foreach (var bar in _barViews)
      Destroy(bar.gameObject);

    _barViews.Clear();

    for (int i = 0; i < bars.Count; i++)
    {
      var barView = Instantiate(barPrefab, contentParent);
      barView.Initialize(_iconDatabase);
      _barViews.Add(barView);
    }

    for (int i = 0; i < _barViews.Count; i++)
    {
      var model = bars[i];

      _barViews[i].SetName(model.upgradeName);
      _barViews[i].SetSlots(model.slotViewModels);
    }
  }
}