using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private PlayerInventory _playerInventory;

    // UI Views
    private IInventoryView _hotbarView;
    private IInventoryView _mainInventoryView;

    private IDragGlost _dragGhost;

    // Hotbar selector (scroll/select)
    private HotbarController _hotbarController;

    private IItemInstance draggedItem;
    private int draggedAmount;
    private InventorySlot draggedOriginalSlot;

    private int hoveredSlot = -1;
    private int draggingSlotIndex = -1;
    private bool draggingFromHotbar = false;

    [Header("Root parents for UI switching")]
    [SerializeField] private Transform gameplayHotbarRoot;
    [SerializeField] private Transform inventoryHotbarRoot;

    private bool _inventoryOpen = false;

    public void Initialize(
        IInventoryView hotbarView,
        IInventoryView mainInventoryView,
        IDragGlost dragGhost,
        HotbarController hotbarController,
        PlayerInventory playerInventory)
    {
        _hotbarView = hotbarView;
        _mainInventoryView = mainInventoryView;

        _dragGhost = dragGhost;

        _hotbarController = hotbarController;
        _playerInventory = playerInventory;

        // HOTBAR UI
        _hotbarController.SetTotalSlots(_playerInventory.Hotbar.Capacity);
        _hotbarView.CreateSlots(_playerInventory.Hotbar.Capacity);

        _hotbarView.OnSlotClicked += _playerInventory.HotbarState.SelectSlot;
        _hotbarController.SlotSelected += _playerInventory.HotbarState.SelectSlot;
        _playerInventory.HotbarState.OnSlotChanged += _hotbarView.SelectSlot;

        _hotbarView.SetMode(InventoryViewMode.Hotbar);
        _mainInventoryView.SetMode(InventoryViewMode.GridInventory);

        // MAIN INVENTORY UI
        _mainInventoryView.CreateSlots(_playerInventory.MainInventory.Capacity);
        _mainInventoryView.OnSlotClicked += OnMainInventorySlotClicked;

        _hotbarView.OnSlotDragEnter += OnHoverHotbarSlotDuringDrag;
        _mainInventoryView.OnSlotDragEnter += OnHoverMainSlotDuringDrag;

        _hotbarView.OnSlotBeginDrag += index =>
        {
            draggingSlotIndex = index;
            draggingFromHotbar = true;

            StartDraggingGhost(_playerInventory.Hotbar.Slots[index]);
        };

        _mainInventoryView.OnSlotBeginDrag += index =>
        {
            draggingSlotIndex = index;
            draggingFromHotbar = false;

            StartDraggingGhost(_playerInventory.MainInventory.Slots[index]);
        };

        _hotbarView.OnSlotBeginDrag += OnBeginDragHotbar;
        _mainInventoryView.OnSlotBeginDrag += OnBeginDragMain;

        _hotbarView.OnSlotDragEnter += OnDragEnterHotbar;
        _mainInventoryView.OnSlotDragEnter += OnDragEnterMain;

        _hotbarView.OnSlotDropOn += OnDropHotbar;
        _mainInventoryView.OnSlotDropOn += OnDropMain;

        _hotbarView.OnSlotEndDrag += index => EndDrag();
        _mainInventoryView.OnSlotEndDrag += index => EndDrag();

        _hotbarView.OnSlotBeginDrag += index =>
        {
            draggingSlotIndex = index;
            draggingFromHotbar = true;
        };

        _mainInventoryView.OnSlotBeginDrag += index =>
        {
            draggingSlotIndex = index;
            draggingFromHotbar = false;
        };

        // Bind Hotbar slot updates
        foreach (var slot in _playerInventory.Hotbar.Slots)
            slot.OnSlotChanged += HandleHotbarSlotChanged;

        // Bind Main inventory slot updates
        foreach (var slot in _playerInventory.MainInventory.Slots)
            slot.OnSlotChanged += HandleMainInventorySlotChanged;

        RefreshAllSlots();

        _hotbarView.Root.transform.SetParent(gameplayHotbarRoot, false);
    }

    private void OnDisable()
    {
        _hotbarView.OnSlotClicked -= _playerInventory.HotbarState.SelectSlot;
        _hotbarController.SlotSelected -= _playerInventory.HotbarState.SelectSlot;
        _mainInventoryView.OnSlotClicked -= OnMainInventorySlotClicked;
    }

    private void OnDestroy()
    {
        foreach (var slot in _playerInventory.Hotbar.Slots)
            slot.OnSlotChanged -= HandleHotbarSlotChanged;

        foreach (var slot in _playerInventory.MainInventory.Slots)
            slot.OnSlotChanged -= HandleMainInventorySlotChanged;
    }

    // --- UPDATE UI WHEN SLOT CHANGES --- //

    private void HandleHotbarSlotChanged(InventorySlot slot)
    {
        int i = _playerInventory.Hotbar.Slots.IndexOf(slot);
        _hotbarView.UpdateSlot(i, SlotToData(slot));
    }
    private void HandleMainInventorySlotChanged(InventorySlot slot)
    {
        int i = _playerInventory.MainInventory.Slots.IndexOf(slot);
        _mainInventoryView.UpdateSlot(i, SlotToData(slot));
    }

    private SlotDisplayData SlotToData(InventorySlot slot)
    {
        var display = new SlotDisplayData();
        if (!slot.IsEmpty)
        {
            display.Icon = slot.Item.Data.Icon;
            display.Amount = slot.Amount;
        }
        return display;
    }

    // --- WHEN USER CLICKS MAIN INVENTORY --- //

    private void OnMainInventorySlotClicked(int mainSlotIndex)
    {
        int hotbarSlotIndex = _playerInventory.MainInventory.Slots.FindIndex(s => s.IsEmpty);
        if (hotbarSlotIndex >= 0)
        {
            _playerInventory.MoveFromInventoryToHotbar(mainSlotIndex, hotbarSlotIndex);
        }
        else
        {
            Debug.Log("⚠ No empty Hotbar slot " + hotbarSlotIndex);
        }
    }

    private void OnBeginDragHotbar(int index)
    {
        draggingSlotIndex = index;
        draggingFromHotbar = true;
        StartDraggingGhost(_playerInventory.Hotbar.Slots[index]);
    }

    private void OnBeginDragMain(int index)
    {
        draggingSlotIndex = index;
        draggingFromHotbar = false;
        StartDraggingGhost(_playerInventory.MainInventory.Slots[index]);
    }
    private void OnDragEnterHotbar(int targetIndex)
    {
        if (draggingSlotIndex < 0) return;

        if (draggingFromHotbar)
            _playerInventory.SwapHotbarSlots(draggingSlotIndex, targetIndex);
        else
            _playerInventory.MoveFromInventoryToHotbar(draggingSlotIndex, targetIndex);

        draggingSlotIndex = targetIndex;
    }

    private void OnDragEnterMain(int targetIndex)
    {
        if (draggingSlotIndex < 0) return;

        if (!draggingFromHotbar)
            _playerInventory.SwapMainSlots(draggingSlotIndex, targetIndex);
        else
            _playerInventory.MoveHotbarToInventory(draggingSlotIndex, targetIndex);

        draggingSlotIndex = targetIndex;
    }

    private void OnDropHotbar(int targetIndex)
    {
        if (draggedItem == null) return;

        int finalTarget = hoveredSlot >= 0 ? hoveredSlot : targetIndex;

        InventorySlot target = _playerInventory.Hotbar.Slots[finalTarget];

        HandleDrop(target);
        EndDrag();
    }

    private void OnDropMain(int targetIndex)
    {
        if (draggedItem == null) return;

        InventorySlot target = _playerInventory.MainInventory.Slots[targetIndex];
        HandleDrop(target);

        EndDrag();
    }

    // -----------------------------------------------------
    // HOTBAR MOVE BETWEEN GAMEPLAY ↔ INVENTORY
    // -----------------------------------------------------
    public void OpenInventory()
    {
        _inventoryOpen = true;

        _hotbarView.Root.transform.SetParent(inventoryHotbarRoot, false);
        _hotbarView.SetMode(InventoryViewMode.GridInventory);

        _mainInventoryView.Root.SetActive(true);
    }

    public void CloseInventory()
    {
        _inventoryOpen = false;

        _hotbarView.Root.transform.SetParent(gameplayHotbarRoot, false);
        _hotbarView.Root.transform.position = Vector3.zero;
        _hotbarView.SetMode(InventoryViewMode.Hotbar);

        _mainInventoryView.Root.SetActive(false);
    }

    // --- PUBLIC REFRESH --- //

    public void RefreshAllSlots()
    {
        // Hotbar
        var hotbarData = new List<SlotDisplayData>();
        foreach (var slot in _playerInventory.Hotbar.Slots)
            hotbarData.Add(SlotToData(slot));
        _hotbarView.UpdateAllSlots(hotbarData);

        // Main Inventory
        var mainData = new List<SlotDisplayData>();
        foreach (var slot in _playerInventory.MainInventory.Slots)
            mainData.Add(SlotToData(slot));
        _mainInventoryView.UpdateAllSlots(mainData);
    }

    private void StartDraggingGhost(InventorySlot slot)
    {
        if (slot.IsEmpty)
        {
            _dragGhost.Hide();
            return;
        }

        _dragGhost.Show(slot.Item.Data.Icon);
    }

    private void StopDraggingGhost()
    {
        _dragGhost.Hide();
    }

    private void OnHoverHotbarSlotDuringDrag(int targetIndex)
    {
        if (draggingSlotIndex < 0) return;

        hoveredSlot = targetIndex;

        if (draggingFromHotbar)
        {
            _playerInventory.SwapHotbarSlots(draggingSlotIndex, targetIndex);
        }
        else
        {
            _playerInventory.MoveFromInventoryToHotbar(draggingSlotIndex, targetIndex);
        }

        draggingSlotIndex = targetIndex; // ⭐ update dragging reference
    }

    private void OnHoverMainSlotDuringDrag(int targetIndex)
    {
        if (draggingSlotIndex < 0) return;

        if (!draggingFromHotbar)
        {
            _playerInventory.SwapMainSlots(draggingSlotIndex, targetIndex);
        }
        else
        {
            _playerInventory.MoveHotbarToInventory(draggingSlotIndex, targetIndex);
        }

        draggingSlotIndex = targetIndex; // ⭐ update dragging reference
    }

    private void EndDrag()
    {
        draggedItem = null;
        draggedAmount = 0;
        draggedOriginalSlot = null;
        draggingSlotIndex = -1;

        _dragGhost.Hide();
    }

    private void HandleDrop(InventorySlot target)
    {
        if (target == draggedOriginalSlot)
        {
            // dropped on the same slot → just return
            draggedOriginalSlot.SetItem(draggedItem, draggedAmount);
            return;
        }

        if (target.IsEmpty)
        {
            // drop on empty → revert
            draggedOriginalSlot.SetItem(draggedItem, draggedAmount);
            return;
        }

        // swapped
        IItemInstance oldItem = target.Item;
        int oldAmount = target.Amount;

        target.SetItem(draggedItem, draggedAmount);
        draggedOriginalSlot.SetItem(oldItem, oldAmount);
    }

    // -------------------------------------------------------
    // --- END DRAG ---
    // -------------------------------------------------------
    private void OnEndDrag(int index)
    {
        if (draggedItem != null)
        {
            // drop outside UI → return
            draggedOriginalSlot.SetItem(draggedItem, draggedAmount);
            EndDrag();
        }
    }

    public void MockInstall(List<IItemData> testItems)
    {
        if (testItems == null) return;
        foreach (var mockItem in testItems)
        {
            if (mockItem.Type == EItemType.Plant)
                _playerInventory.AddItem(new PlantItemInstance(mockItem), 5);
            else if (mockItem.Type == EItemType.Tool)
                _playerInventory.AddItem(new ToolItemInstance(mockItem), 1);
            else if (mockItem.Type == EItemType.Building)
                _playerInventory.AddItem(new BuildingItemInstance(mockItem), 5);
            else if (mockItem.Type == EItemType.Seed)
                _playerInventory.AddItem(new SeedItemInstance(mockItem), 5);
        }
    }
}