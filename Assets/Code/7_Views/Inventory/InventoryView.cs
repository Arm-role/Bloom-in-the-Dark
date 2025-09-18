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

    public void CreateSlots(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            var slotGO = Instantiate(slotPrefab, contentParent);
            var slotView = slotGO.GetComponent<SlotView>();
            slotView.Initialize(i);
            slotView.OnSlotClicked += (view) => OnSlotClicked?.Invoke(view.SlotIndex);
            _slotViews.Add(slotView);
        }

        SelectSlot(0);
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
                slotView.Selected(selectColor);
            }
            else
            {
                slotView.UnSelected(unSelectColor);
            }
        }
    }
}