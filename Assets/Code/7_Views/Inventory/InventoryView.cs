using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour, IInventoryView
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform contentParent;

    [Header("Color")]
    [SerializeField] private Color selectColor;
    [SerializeField] private Color unSelectColor;

    private List<SlotView> _slotViews = new List<SlotView>();

    public event Action<int> OnSlotClicked;
    public event Action<int> OnSlotHovered;
    public event Action<int> OnSlotDraggedOver;

    public InventoryViewMode Mode { get; private set; }
    public GameObject Root => gameObject;

    public void SetMode(InventoryViewMode mode)
    {
        Mode = mode;
    }

    public void CreateSlots(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            var slotGO = Instantiate(slotPrefab, contentParent);
            var slotView = slotGO.GetComponent<SlotView>();
            slotView.Initialize(i);

            slotView.OnClicked += HandleSlotClicked;
            slotView.OnHovered += HandleSlotHovered;
            slotView.OnDraggedOver += HandleSlotDraggedOver;

            _slotViews.Add(slotView);
        }

        if (Mode == InventoryViewMode.GamePlay)
            SelectSlot(0);
        else
            UnselectAll();
    }

    private void HandleSlotClicked(int slotIndex)
    {
        OnSlotClicked?.Invoke(slotIndex);
    }
    private void HandleSlotHovered(int index)
    {
        OnSlotHovered?.Invoke(index);
    }

    private void HandleSlotDraggedOver(int slotIndex)
    {
        OnSlotDraggedOver?.Invoke(slotIndex);
    }

    public void UpdateAllSlots(List<SlotDisplayData> allSlotData)
    {
        for (int i = 0; i < _slotViews.Count; i++)
        {
            if (i < allSlotData.Count)
                UpdateSlot(i, allSlotData[i]);
            else
                _slotViews[i].Clear();
        }
    }

    public void UpdateSlot(int index, SlotDisplayData slotData)
    {
        if (index >= 0 && index < _slotViews.Count)
        {
            _slotViews[index].UpdateView(slotData.Icon, slotData.Amount);
        }
    }
    public void SelectSlot(int slotIndex)
    {
        if(_slotViews.Count < slotIndex) return;

        foreach (var slotView in _slotViews)
        {
            if (slotView.SlotIndex == slotIndex)
            {
                slotView.Select(selectColor);
            }
            else
            {
                slotView.Unselect(unSelectColor);
            }
        }
    }
    public void UnselectAll()
    {
        foreach (var slot in _slotViews)
            slot.Unselect(unSelectColor);
    }
}
