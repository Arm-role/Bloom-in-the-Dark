using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryView : MonoBehaviour, IInventoryView
{
  [SerializeField] private SlotView slotPrefab;
  [SerializeField] private Transform contentParent;
  [SerializeField] private Color selectedColor;
  [SerializeField] private Color normalColor;

  private readonly List<SlotView> _slotViews = new();

  private IItemIconProvider _iconDatabase;

  public event Action<int> OnSlotClicked;
  public event Action<int> OnSlotHovered;
  public event Action<int> OnSlotDraggedOver;

  // =============================
  // Initialization
  // =============================

  public void Initialize(
    IItemIconProvider itemIconDatabase)
  {
    _iconDatabase = itemIconDatabase;
  }

  public void CreateSlots(int capacity)
  {
    Debug.Log("Create");

    for (int i = 0; i < capacity; i++)
    {
      var slotView = Instantiate(slotPrefab, contentParent);

      slotView.Initialize(i);

      slotView.OnClicked += i => OnSlotClicked?.Invoke(i);
      slotView.OnHovered += i => OnSlotHovered?.Invoke(i);
      slotView.OnDraggedOver += i => OnSlotDraggedOver?.Invoke(i);

      _slotViews.Add(slotView);
    }
  }

  // =============================
  // Rendering
  // =============================

  public void SetSlots(IReadOnlyList<SlotViewModel> slots)
  {
    for (int i = 0; i < _slotViews.Count; i++)
    {
      if (i >= slots.Count)
      {
        _slotViews[i].Clear();
        continue;
      }

      var model = slots[i];

      if (model.IsEmpty)
      {
        _slotViews[i].Clear();
        continue;
      }

      _slotViews[i].SetAmount(model.Amount);
      _slotViews[i].SetIcon(ResolveIcon(model.ItemId));
    }
  }

  // =============================
  // Select
  // =============================

  public void Highlight(int index)
  {
    for (int i = 0; i < _slotViews.Count; i++)
    {
      if (i == index)
        _slotViews[i].SetHighlight(selectedColor);
      else
        _slotViews[i].SetHighlight(normalColor);
    }
  }

  // =============================
  // Cooldown
  // =============================

  public void ShowCooldown(int index, float remaining, float normalized)
  {
    if (index < 0 || index >= _slotViews.Count)
      return;

    _slotViews[index].ShowCooldown(remaining, normalized);
  }

  public void HideCooldown(int index)
  {
    if (index < 0 || index >= _slotViews.Count)
      return;

    _slotViews[index].HideCooldown();
  }
  // =============================
  // Presentation-only Helpers
  // =============================

  private Sprite ResolveIcon(int itemId)
  {
    return _iconDatabase.GetIcon(itemId);
  }
}