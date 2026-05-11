using UnityEngine;
using System.Collections.Generic;

public sealed class InventoryController
{
  private readonly IInventoryView _hotbarView;
  private readonly IInventoryView _mainView;
  private readonly HotbarState _hotbarState;
  private readonly InventoryService _service;
  private readonly IDragGhost _dragGhost;
  private readonly IItemIconProvider _iconDatabase;
  private readonly ITooltipService _tooltip;

  private InventorySide _hoveredSide;

  private int _gameplayHotbarSlot = 0;
  private int _hoveredIndex = -1;

  private bool _isInventoryOpen;


  public InventoryController(
      IInventoryView hotbarView,
      IInventoryView mainView,
      HotbarState state,
      InventoryService service,
      IDragGhost dragGhost,
      IItemIconProvider iconDatabase,
      ITooltipService tooltip)
  {
    _hotbarView = hotbarView;
    _mainView = mainView;
    _hotbarState = state;
    _service = service;
    _dragGhost = dragGhost;
    _iconDatabase = iconDatabase;
    _tooltip = tooltip;

    state.OnSlotChanged += HighlightSelectSlot;
  }

  public void Initialize()
  {
    BindViewEvents();
    BindSlotDataEvents();
    BindDomainEvents();
    RefreshAll();

    HighlightSelectSlot(_hotbarState.CurrentSlotIndex);
  }

  // =============================
  // Binding
  // =============================

  private void BindViewEvents()
  {
    _hotbarView.OnSlotClicked += i => HandleClick(InventorySide.Hotbar, i);
    _mainView.OnSlotClicked += i => HandleClick(InventorySide.Main, i);

    _hotbarView.OnSlotHovered += i => HandleHover(InventorySide.Hotbar, i);
    _mainView.OnSlotHovered += i => HandleHover(InventorySide.Main, i);

    _hotbarView.OnSlotDraggedOver += i => HandleDragOver(InventorySide.Hotbar, i);
    _mainView.OnSlotDraggedOver += i => HandleDragOver(InventorySide.Main, i);

    _hotbarView.OnSlotExited += _ => _tooltip.Hide();
    _mainView.OnSlotExited += _ => _tooltip.Hide();

    _hotbarView.CreateSlots(_service.GetHotbarSlots().Count);
    _mainView.CreateSlots(_service.GetMainSlots().Count);
  }

  private void BindSlotDataEvents()
  {
    foreach (var slot in _service.GetHotbarSlots())
      slot.OnSlotChanged += _ => RefreshHotbar();

    foreach (var slot in _service.GetMainSlots())
      slot.OnSlotChanged += _ => RefreshMain();
  }

  private void BindDomainEvents()
  {
    _service.OnInventoryChanged += RefreshAll;
    _service.OnInventoryChanged += RefreshHoverTooltip;
  }

  // =============================
  // Click / Hover
  // =============================

  private void HandleClick(InventorySide side, int index)
  {
    if (!_isInventoryOpen && side == InventorySide.Hotbar)
    {
      _hotbarState.SelectSlot(index);
      return;
    }

    var result = _service.HandleClick(
    side,
    index,
    new InventoryClickContext
    {
      IsShift = Input.GetKey(KeyCode.LeftShift)
    });

    ClearInventoryHover();

    if (result.IsHolding)
    {
      var icon = _iconDatabase.GetIcon(result.Item.Data.ID);
      _dragGhost.Show(icon, result.Amount);
    }
    else
    {
      _dragGhost.Hide();
    }
  }

  private void HandleHover(InventorySide side, int index)
  {
    if (!_isInventoryOpen) return;
    if (_hoveredSide == side && _hoveredIndex == index) return;

    _hoveredSide = side;
    _hoveredIndex = index;

    if (side == InventorySide.Hotbar) { _hotbarView.Highlight(index); _mainView.Highlight(-1); }
    else { _mainView.Highlight(index); _hotbarView.Highlight(-1); }

    var slots = side == InventorySide.Hotbar
        ? _service.GetHotbarSlots()
        : _service.GetMainSlots();

    if (index < slots.Count && !slots[index].IsEmpty)
      _tooltip.Show(BuildTooltip(slots[index]));
    else
      _tooltip.Hide();
  }

  private void ClearInventoryHover()
  {
    _hoveredIndex = -1;
  }

  private void RefreshHoverTooltip()
  {
    if (_hoveredIndex < 0) return;

    var slots = _hoveredSide == InventorySide.Hotbar
        ? _service.GetHotbarSlots()
        : _service.GetMainSlots();

    if (_hoveredIndex < slots.Count && !slots[_hoveredIndex].IsEmpty)
      _tooltip.Show(BuildTooltip(slots[_hoveredIndex]));
    else
      _tooltip.Hide();
  }

  private void HandleDragOver(InventorySide side, int index)
  {
    _service.HandleDragOver(side, index, Input.GetKey(KeyCode.LeftShift), Input.GetMouseButtonDown(0));
  }

  // =============================
  // View
  // =============================

  public void OnInventoryOpened()
  {
    _isInventoryOpen = true;

    _gameplayHotbarSlot = _hotbarState.CurrentSlotIndex;
    _dragGhost.Active();
    _dragGhost.Hide();

    RefreshSelection();
    RefreshAll();
  }

  public void OnInventoryClosed()
  {
    _isInventoryOpen = false;
    _tooltip.Hide();

    _hotbarState.SelectSlot(_gameplayHotbarSlot);
    _dragGhost.UnActive();

    ClearInventoryHover();
    RefreshAll();
  }

  // =============================
  // Tooltip
  // =============================

  private TooltipData BuildTooltip(InventorySlot slot)
  {
    var instance = slot.GetItemInstance();
    var def      = instance.Data;

    var desc = $"{def.Role}  X{slot.Amount}";
    if (def.MaxStackSize > 1) desc += $"/{def.MaxStackSize}";

    var profile = def.InteractionProfile;
    if (profile != null)
    {
      if (profile.Damage > 0) desc += $"\nDamage: {profile.Damage:0.#}";
      if (profile.Range  > 0) desc += $"\nRange: {profile.Range:0.#}";
    }

    return new TooltipData(def.Name, desc);
  }

  // =============================
  // Refresh
  // =============================

  private void RefreshAll()
  {
    RefreshHotbar();
    RefreshMain();
  }

  private void RefreshHotbar()
  {
    var domainSlots = _service.GetHotbarSlots();
    Debug.Log(domainSlots.Count);
    var models = BuildViewModels(domainSlots);

    _hotbarView.SetSlots(models);
  }

  private void RefreshMain()
  {
    var domainSlots = _service.GetMainSlots();
    Debug.Log(domainSlots.Count);
    var models = BuildViewModels(domainSlots);

    _mainView.SetSlots(models);
  }
  private void HighlightSelectSlot(int index)
  {
    if (_isInventoryOpen) return;
    _hotbarView.Highlight(index);
  }

  public void RefreshSelection()
  {
    _hotbarView.Highlight(_hotbarState.CurrentSlotIndex);

    _mainView.Highlight(-1);
  }

  private List<SlotViewModel> BuildViewModels(
      IReadOnlyList<InventorySlot> slots)
  {
    var list = new List<SlotViewModel>(slots.Count);

    foreach (var slot in slots)
    {
      if (slot.IsEmpty)
      {
        list.Add(new SlotViewModel
        {
          IsEmpty = true
        });
        continue;
      }

      list.Add(new SlotViewModel
      {
        ItemId = slot.ItemId,
        DisplayName = slot.DisplayName,
        Amount = slot.Amount,
        IsEmpty = false
      });
    }

    return list;
  }
}