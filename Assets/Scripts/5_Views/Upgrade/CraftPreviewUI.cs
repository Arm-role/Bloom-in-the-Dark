using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftPreviewUI : MonoBehaviour
{
  [SerializeField] private GameObject _panel;
  [SerializeField] private TMP_Text _upgradeNameText;
  [SerializeField] private UpgradeRequestSlotView _slotPrefab;
  [SerializeField] private Transform _slotsParent;
  [SerializeField] private Button _confirmButton;

  private readonly List<UpgradeRequestSlotView> _slotViews = new();
  private IItemIconProvider _iconProvider;
  private Action _onConfirm;

  public void Initialize(IItemIconProvider iconProvider)
  {
    _iconProvider = iconProvider;
    _confirmButton.onClick.AddListener(OnConfirmClicked);
    Hide();
  }

  public void Show(UpgradeRequestDefinition request, Action onConfirm)
  {
    _onConfirm = onConfirm;
    _upgradeNameText.text = request.UpgradeName;

    ClearSlots();

    foreach (var ingredient in request.Ingredients)
    {
      var slot = Instantiate(_slotPrefab, _slotsParent);
      slot.SetIcon(_iconProvider.GetIcon(ingredient.item.RuntimeTag.Hash));
      slot.SetAmount(ingredient.amount, ingredient.amount);
      _slotViews.Add(slot);
    }

    _panel.SetActive(true);
  }

  public void Hide()
  {
    _panel.SetActive(false);
    ClearSlots();
    _onConfirm = null;
  }

  private void OnConfirmClicked()
  {
    _onConfirm?.Invoke();
  }

  private void ClearSlots()
  {
    foreach (var slot in _slotViews)
      Destroy(slot.gameObject);
    _slotViews.Clear();
  }
}
