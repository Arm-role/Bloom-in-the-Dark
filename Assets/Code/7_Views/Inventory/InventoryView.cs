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
    public event Action<int> OnSlotBeginDrag;
    public event Action<int> OnSlotEndDrag;
    public event Action<int> OnSlotDropOn;
    public event Action<int> OnSlotDragEnter;
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

            // Click
            slotView.OnSlotClicked += (view) => OnSlotClicked?.Invoke(view.SlotIndex);

            // NEW: Drag Start
            slotView.OnBeginDragEvent += (index) => OnSlotBeginDrag?.Invoke(index);

            // NEW: Drop
            slotView.OnDropOnEvent += (index) => OnSlotDropOn?.Invoke(index);

            slotView.OnDragEnterEvent += index => OnSlotDragEnter?.Invoke(index);

            _slotViews.Add(slotView);
        }

        if (Mode == InventoryViewMode.Hotbar)
            SelectSlot(0);
        else
            UnselectAll();
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
    public void UnselectAll()
    {
        foreach (var slot in _slotViews)
            slot.UnSelected(unSelectColor);
    }
}
