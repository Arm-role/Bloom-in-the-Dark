using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Transform inventoryRoot;
    [SerializeField] private Transform hotbarRoot;

    [SerializeField] private GameObject energyBar;
    // ==============================
    // Dependencies
    // ==============================
    private PlayerInventory _playerInventory;
    private HotbarState _hotbarState;
    private IInventoryView _hotbarView;
    private IInventoryView _mainView;
    private IDragGlost _dragGhost;

    // ==============================
    // Drag Context
    // ==============================
    private InventoryDragContext _dragContext;

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

        _hotbarView.SetMode(InventoryViewMode.Hotbar);
        _hotbarView.CreateSlots(_playerInventory.Hotbar.Capacity);

        _mainView.SetMode(InventoryViewMode.GridInventory);
        _mainView.CreateSlots(_playerInventory.MainInventory.Capacity);
    }

    private void OnDestroy()
    {
        if (_hotbarState != null)
            _hotbarState.OnSlotChanged -= _hotbarView.SelectSlot;
    }

    private void BindSlotEvents()
    {
        // ---- Click ----
        _hotbarView.OnSlotClicked += i => OnSlotClicked(InventorySide.Hotbar, i);
        _mainView.OnSlotClicked += i => OnSlotClicked(InventorySide.Main, i);

        // ---- Drag ----
        _hotbarView.OnSlotBeginDrag += i => BeginDrag(InventorySide.Hotbar, i);
        _mainView.OnSlotBeginDrag += i => BeginDrag(InventorySide.Main, i);

        _hotbarView.OnSlotDragEnter += i => DragEnter(InventorySide.Hotbar, i);
        _mainView.OnSlotDragEnter += i => DragEnter(InventorySide.Main, i);

        _hotbarView.OnSlotDropOn += i => DropOn(InventorySide.Hotbar, i);
        _mainView.OnSlotDropOn += i => DropOn(InventorySide.Main, i);

        _hotbarView.OnSlotEndDrag += _ => EndDrag();
        _mainView.OnSlotEndDrag += _ => EndDrag();
    }

    private void BindSlotDataEvents()
    {
        foreach (var slot in _playerInventory.Hotbar.Slots)
            slot.OnSlotChanged += _ => RefreshHotbar();

        foreach (var slot in _playerInventory.MainInventory.Slots)
            slot.OnSlotChanged += _ => RefreshMain();
    }

    // ==============================
    // Click (Shift Shortcut)
    // ==============================
    private void OnSlotClicked(InventorySide side, int index)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        var fromInv = GetInventory(side);
        var toInv = GetInventory(side == InventorySide.Hotbar
            ? InventorySide.Main
            : InventorySide.Hotbar);

        var source = fromInv.Slots[index];
        if (source.IsEmpty) return;

        int targetIndex = toInv.Slots.FindIndex(s => s.IsEmpty);
        if (targetIndex < 0) return;

        toInv.Slots[targetIndex].SetItem(source.Item, source.Amount);
        source.Clear();

        Debug.Log("OnSlotClicked");
    }

    // ==============================
    // Drag Flow
    // ==============================
    private void BeginDrag(InventorySide side, int index)
    {
        var slot = GetInventory(side).Slots[index];
        if (slot.IsEmpty) return;

        _dragContext = new InventoryDragContext
        {
            IsDragging = true,
            SourceSide = side,
            SourceIndex = index,
            SourceSlot = slot,
            TargetSide = side,
            TargetIndex = index
        };

        _dragGhost.Show(slot.Item.Data.Icon);
        Debug.Log("BeginDrag");
    }

    private void DragEnter(InventorySide side, int index)
    {
        if (!_dragContext.IsDragging) return;

        _dragContext.TargetSide = side;
        _dragContext.TargetIndex = index;
        Debug.Log("DragEnter");
    }

    private void DropOn(InventorySide side, int index)
    {
        if (!_dragContext.IsDragging) return;

        _dragContext.TargetSide = side;
        _dragContext.TargetIndex = index;

        ResolveDrop(_dragContext);
        EndDrag();
        Debug.Log("DropOn");
    }

    private void EndDrag()
    {
        _dragContext.Clear();
        _dragGhost.Hide();
    }

    // ==============================
    // Drop Resolution (THE CORE)
    // ==============================
    private void ResolveDrop(InventoryDragContext ctx)
    {
        var sourceInv = GetInventory(ctx.SourceSide);
        var targetInv = GetInventory(ctx.TargetSide);

        var sourceSlot = sourceInv.Slots[ctx.SourceIndex];
        var targetSlot = targetInv.Slots[ctx.TargetIndex];

        // same slot → do nothing
        if (ctx.SourceSide == ctx.TargetSide &&
            ctx.SourceIndex == ctx.TargetIndex)
            return;

        // move
        if (targetSlot.IsEmpty)
        {
            targetSlot.SetItem(sourceSlot.Item, sourceSlot.Amount);
            sourceSlot.Clear();
            return;
        }

        // swap
        var tempItem = targetSlot.Item;
        var tempAmount = targetSlot.Amount;

        targetSlot.SetItem(sourceSlot.Item, sourceSlot.Amount);
        sourceSlot.SetItem(tempItem, tempAmount);
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
        _hotbarView.SetMode(InventoryViewMode.GridInventory);
        _mainView.Root.SetActive(true);
        _hotbarView.Root.transform.SetParent(inventoryRoot);
        energyBar.SetActive(false);
        _dragGhost.Active();

        EndDrag();
        RefreshAll();
    }

    public void CloseInventory()
    {
        _hotbarView.SetMode(InventoryViewMode.Hotbar);
        _mainView.Root.SetActive(false);
        _hotbarView.Root.transform.SetParent(hotbarRoot);
        energyBar.SetActive(true);
        _dragGhost.UnActive();

        EndDrag();
    }
}
