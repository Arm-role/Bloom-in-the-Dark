using System;
using System.Collections.Generic;
using UnityEngine;

public class TradeView : MonoBehaviour, ITradeView
{
  [SerializeField] private GameObject _panelRoot;
  [SerializeField] private Transform _offerContainer;
  [SerializeField] private TradeOfferRow _offerRowPrefab;

  public event Action<int> OnOfferClicked;

  private IItemIconProvider _icons;
  private readonly List<TradeOfferRow> _rows = new();

  public void Initialize(IItemIconProvider icons)
  {
    _icons = icons;
    _panelRoot.SetActive(false);
  }

  public void Open(IReadOnlyList<TradeOfferDisplay> offers)
  {
    _panelRoot.SetActive(true);
    Rebuild(offers);
  }

  // จำนวน offer คงที่ระหว่าง session (แลกได้ไม่จำกัด) — refresh แค่ rebind count/affordable
  public void Refresh(IReadOnlyList<TradeOfferDisplay> offers)
  {
    for (int i = 0; i < _rows.Count && i < offers.Count; i++)
      _rows[i].Bind(i, offers[i], _icons);
  }

  public void Close()
  {
    _panelRoot.SetActive(false);
  }

  private void Rebuild(IReadOnlyList<TradeOfferDisplay> offers)
  {
    ClearRows();

    for (int i = 0; i < offers.Count; i++)
    {
      var row = Instantiate(_offerRowPrefab, _offerContainer);
      row.OnClicked += HandleRowClicked;
      row.Bind(i, offers[i], _icons);
      _rows.Add(row);
    }
  }

  private void ClearRows()
  {
    foreach (var row in _rows)
    {
      if (row == null) continue;
      row.OnClicked -= HandleRowClicked;
      Destroy(row.gameObject);
    }
    _rows.Clear();
  }

  private void HandleRowClicked(int index) => OnOfferClicked?.Invoke(index);
}
