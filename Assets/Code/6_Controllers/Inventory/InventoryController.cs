using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Transform inventoryRoot;
    [SerializeField] private Transform hotbarRoot;

    [SerializeField] private GameObject energyBar;

    // ==============================
    // Dependencies
    // ==============================
    private int _gameplayHotbarSlot = 0;

    // Inventory UI
    private InventorySide _hoveredSide;
    private int _hoveredIndex = -1;

    private PlayerInventory _playerInventory;
    private HotbarState _hotbarState;
    private IInventoryView _hotbarView;
    private IInventoryView _mainView;
    private IDragGlost _dragGhost;

    // ==============================
    // Drag Context
    // ==============================

    private InventoryPickContext _pickContext = new InventoryPickContext();

    private HashSet<(InventorySide, int)> _sweepedSlots = new();
    // ==============================
    // Initialization
    // ==============================

    public void Initialize(
        PlayerInventory playerInventory,
        HotbarState hotbarState,
        IInventoryView hotbarView,
        IInventoryView mainView,
        IDragGlost dragGhost)
    {
        _playerInventory = playerInventory;
        _hotbarState = hotbarState;
        _hotbarView = hotbarView;
        _mainView = mainView;
        _dragGhost = dragGhost;

        SetupViews();
        BindSlotEvents();
        BindSlotDataEvents();

        RefreshAll();
    }

    // ==============================
    // Setup
    // ==============================
    private void SetupViews()
    {
        _hotbarState.OnSlotChanged += _hotbarView.SelectSlot;
        _hotbarView.SelectSlot(_hotbarState.CurrentSlotIndex);

        _hotbarView.SetMode(InventoryViewMode.GamePlay);
        _hotbarView.CreateSlots(_playerInventory.Hotbar.Capacity);

        _mainView.SetMode(InventoryViewMode.Inventory);
        _mainView.CreateSlots(_playerInventory.MainInventory.Capacity);
    }

    private void OnDestroy()
    {
        if (_hotbarState != null)
            _hotbarState.OnSlotChanged -= _hotbarView.SelectSlot;
    }

    private void BindSlotEvents()
    {
        _hotbarView.OnSlotClicked += i => OnSlotClicked(InventorySide.Hotbar, i);
        _mainView.OnSlotClicked += i => OnSlotClicked(InventorySide.Main, i);

        _hotbarView.OnSlotHovered += i => OnSlotHovered(InventorySide.Hotbar, i);
        _mainView.OnSlotHovered += i => OnSlotHovered(InventorySide.Main, i);

        _hotbarView.OnSlotDraggedOver += i => OnSlotDraggedOver(InventorySide.Hotbar, i);
        _mainView.OnSlotDraggedOver += i => OnSlotDraggedOver(InventorySide.Main, i);
    }

    private void BindSlotDataEvents()
    {
        foreach (var slot in _playerInventory.Hotbar.Slots)
            slot.OnSlotChanged += _ => RefreshHotbar();

        foreach (var slot in _playerInventory.MainInventory.Slots)
            slot.OnSlotChanged += _ => RefreshMain();
    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetMouseButton(0))
            _sweepedSlots.Clear();
    }

    // ==============================
    // Click (Shift Shortcut)
    // ==============================

    private void OnSlotClicked(InventorySide side, int index)
    {
        if (_hotbarView.Mode == InventoryViewMode.GamePlay)
        {
            HandleGameplaySelect(side, index);
            return;
        }

        if (_pickContext.IsHolding)
            PlaceItem(side, index);
        else
            PickItem(side, index);
    }

    private void OnSlotHovered(InventorySide side, int index)
    {
        if (_hotbarView.Mode == InventoryViewMode.GamePlay)
            return;

        if (_hoveredSide == side && _hoveredIndex == index)
            return;

        _hoveredSide = side;
        _hoveredIndex = index;

        // 🔥 Unselect ทุก view ก่อน
        _hotbarView.UnselectAll();
        _mainView.UnselectAll();

        // 🔥 Select view ที่ถูกต้อง
        if (side == InventorySide.Hotbar)
            _hotbarView.SelectSlot(index);
        else
            _mainView.SelectSlot(index);
    }
    private void ClearInventoryHover()
    {
        _hoveredIndex = -1;
    }

    private void OnSlotDraggedOver(InventorySide side, int index)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        if (_pickContext.IsHolding)
            return;

        if (_sweepedSlots.Contains((side, index)))
            return;

        var slot = GetInventory(side).Slots[index];
        if (slot.IsEmpty)
            return;

        _sweepedSlots.Add((side, index));
        QuickMove(side, index);
    }

    // =======================
    // Pick
    // =======================

    private void PickItem(InventorySide side, int index)
    {
        var slot = GetInventory(side).Slots[index];
        if (slot.IsEmpty) return;

        _pickContext.IsHolding = true;
        _pickContext.SourceSide = side;
        _pickContext.SourceIndex = index;
        _pickContext.Item = slot.Item;
        _pickContext.Amount = slot.Amount;


        _dragGhost.Show(slot.Item.Data.Icon, slot.Amount);

        slot.Clear();
        RefreshAll();
    }

    // =======================
    // Place / Swap
    // =======================

    private void PlaceItem(InventorySide side, int index)
    {
        var targetSlot = GetInventory(side).Slots[index];

        // วางกลับช่องเดิม
        if (_pickContext.SourceSide == side &&
            _pickContext.SourceIndex == index)
        {
            targetSlot.SetItem(_pickContext.Item, _pickContext.Amount);
            EndPick();
            return;
        }

        if (targetSlot.IsEmpty)
        {
            targetSlot.SetItem(_pickContext.Item, _pickContext.Amount);
        }
        else
        {
            // สลับของ
            var tempItem = targetSlot.Item;
            var tempAmount = targetSlot.Amount;

            targetSlot.SetItem(_pickContext.Item, _pickContext.Amount);

            var sourceSlot = GetInventory(_pickContext.SourceSide)
                .Slots[_pickContext.SourceIndex];

            sourceSlot.SetItem(tempItem, tempAmount);
        }

        EndPick();
    }

    private void EndPick()
    {
        Debug.Log("EndPick");

        _pickContext.Clear();
        _dragGhost.Hide();
        RefreshAll();
    }

    // =======================
    // Quick Move (Shift)
    // =======================

    private void QuickMove(InventorySide side, int index)
    {
        var from = GetInventory(side);
        var to = GetInventory(side == InventorySide.Hotbar
            ? InventorySide.Main
            : InventorySide.Hotbar);

        var source = from.Slots[index];
        if (source.IsEmpty) return;

        int targetIndex = to.Slots.FindIndex(s => s.IsEmpty);
        if (targetIndex < 0) return;

        to.Slots[targetIndex].SetItem(source.Item, source.Amount);
        source.Clear();

        RefreshAll();
    }

    private void HandleGameplaySelect(InventorySide side, int index)
    {
        if (side != InventorySide.Hotbar)
            return;

        _hotbarState.SelectSlot(index);
    }

    // ==============================
    // Helpers
    // ==============================

    private InventoryLogic GetInventory(InventorySide side)
    {
        return side == InventorySide.Hotbar
            ? _playerInventory.Hotbar
            : _playerInventory.MainInventory;
    }

    private void RefreshAll()
    {
        RefreshHotbar();
        RefreshMain();
    }

    private void RefreshHotbar()
    {
        var data = new List<SlotDisplayData>();
        foreach (var slot in _playerInventory.Hotbar.Slots)
            data.Add(ToDisplay(slot));

        _hotbarView.UpdateAllSlots(data);
    }

    private void RefreshMain()
    {
        var data = new List<SlotDisplayData>();
        foreach (var slot in _playerInventory.MainInventory.Slots)
            data.Add(ToDisplay(slot));

        _mainView.UpdateAllSlots(data);
    }

    private SlotDisplayData ToDisplay(InventorySlot slot)
    {
        return slot.IsEmpty
            ? new SlotDisplayData()
            : new SlotDisplayData
            {
                Icon = slot.Item.Data.Icon,
                Amount = slot.Amount
            };
    }

    public void OpenInventory()
    {
        _gameplayHotbarSlot = _hotbarState.CurrentSlotIndex;

        _hotbarView.SetMode(InventoryViewMode.Inventory);
        _mainView.Root.SetActive(true);
        _hotbarView.Root.transform.SetParent(inventoryRoot);

        energyBar.SetActive(false);
        _dragGhost.Active();

        EndPick();
        RefreshAll();
    }

    public void CloseInventory()
    {
        _hotbarState.SelectSlot(_gameplayHotbarSlot);
        _hotbarView.SelectSlot(_gameplayHotbarSlot);

        _hotbarView.SetMode(InventoryViewMode.GamePlay);
        _mainView.Root.SetActive(false);
        _hotbarView.Root.transform.SetParent(hotbarRoot);
        energyBar.SetActive(true);
        _dragGhost.UnActive();

        ClearInventoryHover();

        EndPick();
    }
}
