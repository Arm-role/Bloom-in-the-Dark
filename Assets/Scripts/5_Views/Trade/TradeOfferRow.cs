using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 1 แถวของ trade offer — input slots (มี/ต้องการ) → output slot + ปุ่มแลก
public class TradeOfferRow : MonoBehaviour
{
  [SerializeField] private Transform _inputContainer;
  [SerializeField] private TradeItemSlot _slotPrefab;
  [SerializeField] private TradeItemSlot _outputSlot;
  [SerializeField] private Button _tradeButton;

  public event Action<int> OnClicked;

  private int _index;
  private readonly List<TradeItemSlot> _inputSlots = new();

  private void Awake()
  {
    _tradeButton.onClick.AddListener(() => OnClicked?.Invoke(_index));
  }

  private void OnDestroy()
  {
    _tradeButton.onClick.RemoveAllListeners();
  }

  public void Bind(int index, TradeOfferDisplay offer, IItemIconProvider icons)
  {
    _index = index;

    EnsureInputSlots(offer.Inputs.Length);

    for (int i = 0; i < offer.Inputs.Length; i++)
    {
      var input = offer.Inputs[i];
      _inputSlots[i].Bind(
        input.ItemId,
        $"{input.OwnedAmount}/{input.RequiredAmount}",
        input.OwnedAmount >= input.RequiredAmount,
        icons);
    }

    _outputSlot.Bind(offer.Output.ItemId, $"x{offer.Output.RequiredAmount}", true, icons);
    _tradeButton.interactable = offer.CanAfford;
  }

  // สร้าง input slot ครั้งเดียวพอ — จำนวน input ต่อ offer คงที่ ไม่สร้างซ้ำตอน refresh
  private void EnsureInputSlots(int count)
  {
    while (_inputSlots.Count < count)
      _inputSlots.Add(Instantiate(_slotPrefab, _inputContainer));
  }
}
