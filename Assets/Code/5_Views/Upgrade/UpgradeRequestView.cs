using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeRequestView : MonoBehaviour, IUpgradeRequestView
{
  [SerializeField] private UpgradeRequestBarView barPrefab;
  [SerializeField] private GameObject Canvas;
  [SerializeField] private Transform contentParent;

  private readonly List<UpgradeRequestBarView> _barViews = new();

  private IItemIconProvider _iconDatabase;

  public event Action<RequestBarViewModel> OnBarClicked;

  // =============================
  // Initialization
  // =============================

  public void Initialize(IItemIconProvider itemIconDatabase)
  {
    _iconDatabase = itemIconDatabase;
    Hide();
  }

  // =============================
  // Rendering
  // =============================

  public void SetSlots(IReadOnlyList<RequestBarViewModel> bars)
  {
    foreach (var bar in _barViews)
      Destroy(bar.gameObject);

    _barViews.Clear();
    Canvas.SetActive(true);

    for (int i = 0; i < bars.Count; i++)
    {
      var model = bars[i];

      var barView = Instantiate(barPrefab, contentParent);
      barView.Initialize(_iconDatabase);

      barView.Bind(model);

      barView.SelectUpgradeRequest += OnBarClickedInternal;

      barView.SetName(model.upgradeName);
      barView.SetSlots(model.slotViewModels);

      _barViews.Add(barView);
    }
  }

  private void OnBarClickedInternal(RequestBarViewModel model)
  {
    OnBarClicked?.Invoke(model);
  }

  public void Hide()
  {
    Canvas.SetActive(false);
  }
}
