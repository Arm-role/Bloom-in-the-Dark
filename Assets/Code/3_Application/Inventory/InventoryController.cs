using UnityEngine;
using System.Collections.Generic;

public sealed class InventoryController
{
  private readonly IInventoryView _hotbarView;
  private readonly IInventoryView _mainView;
  private readonly HotbarState _hotbarState;
  private readonly InventoryService _service;
  private readonly IDragGlost _dragGhost;
  private readonly CooldownContainer _cooldowns;
  private readonly IItemIconProvider _iconDatabase;

  private InventorySide _hoveredSide;

  private int _gameplayHotbarSlot = 0;
  private int _hoveredIndex = -1;

  private bool _isInventoryOpen;


  public InventoryController(
      IInventoryView hotbarView,
      IInventoryView mainView,
      HotbarState state,
      InventoryService service,
      IDragGlost dragGlost,
      CooldownContainer cooldowns,
      IItemIconProvider iconDatabase)
  {
    _hotbarView = hotbarView;
    _mainView = mainView;
    _hotbarState = state;
    _service = service;
    _dragGhost = dragGlost;
    _cooldowns = cooldowns;

    state.OnSlotChanged += HighlightSelectSlot;
    _iconDatabase = iconDatabase;
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
    _cooldowns.OnCooldownEnded += HideAllCooldownForKey;
  }

  // =============================
  // Tick
  // =============================

  public void Tick()
  {
    _cooldowns.Tick();

    foreach (var key in _cooldowns.ActiveKeys)
    {
      UpdateAllForKey(key);
    }
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
    if (!_isInventoryOpen)
      return;

    if (_hoveredSide == side && _hoveredIndex == index)
      return;

    _hoveredSide = side;
    _hoveredIndex = index;

    if (side == InventorySide.Hotbar)
    {
      _hotbarView.Highlight(index);
      _mainView.Highlight(-1);
    }
    else
    {
      _mainView.Highlight(index);
      _hotbarView.Highlight(-1);
    }
  }
  private void ClearInventoryHover()
  {
    _hoveredIndex = -1;
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

    _hotbarState.SelectSlot(_gameplayHotbarSlot);
    _dragGhost.UnActive();

    ClearInventoryHover();
    RefreshAll();
  }

  // =============================
  // Cooldown
  // =============================

  private void UpdateAllForKey(string key)
  {
    UpdateHotbarForKey(key);
    UpdateMainForKey(key);  
  }

  private void UpdateHotbarForKey(string key)
  {
    var hotbar = _service.GetHotbarSlots();

    for (int i = 0; i < hotbar.Count; i++)
    {
      var slot = hotbar[i];
      if (slot.IsEmpty)
        continue;

      if (slot.DisplayName != key)
        continue;

      if (_cooldowns.TryGetCooldown(slot.DisplayName, out var cd))
      {
        _hotbarView.ShowCooldown(i, cd.Remaining, cd.Normalized);
      }

    }
  }

  private void UpdateMainForKey(string key)
  {
    var main = _service.GetMainSlots();

    for (int i = 0; i < main.Count; i++)
    {
      var slot = main[i];
      if (slot.IsEmpty)
        continue;

      if (slot.DisplayName != key)
        continue;

      if (_cooldowns.TryGetCooldown(slot.DisplayName, out var cd))
      {
        _mainView.ShowCooldown(i, cd.Remaining, cd.Normalized);
      }

    }
  }

  private void HideAllCooldownForKey(string key)
  {
    HideHotbarCooldownForKey(key);
    HideMainCooldownForKey(key);
  }

  private void HideHotbarCooldownForKey(string key)
  {
    var hotbar = _service.GetHotbarSlots();

    for (int i = 0; i < hotbar.Count; i++)
    {
      var slot = hotbar[i];
      if (slot.IsEmpty)
        continue;

      if (slot.DisplayName == key)
      {
        _hotbarView.HideCooldown(i);
      }
    }
  }

  private void HideMainCooldownForKey(string key)
  {
    var main = _service.GetMainSlots();

    for (int i = 0; i < main.Count; i++)
    {
      var slot = main[i];
      if (slot.IsEmpty)
        continue;

      if (slot.DisplayName == key)
      {
        _mainView.HideCooldown(i);
      }
    }
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