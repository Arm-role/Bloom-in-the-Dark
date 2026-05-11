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
  private readonly IStatDatabase _statDatabase;
  private readonly IUpgradeContainer _upgradeContainer;
  private readonly IInteractionCostConfig _costConfig;

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
      ITooltipService tooltip,
      IStatDatabase statDatabase,
      IUpgradeContainer upgradeContainer,
      IInteractionCostConfig costConfig)
  {
    _hotbarView = hotbarView;
    _mainView = mainView;
    _hotbarState = state;
    _service = service;
    _dragGhost = dragGhost;
    _iconDatabase = iconDatabase;
    _tooltip = tooltip;
    _statDatabase = statDatabase;
    _upgradeContainer = upgradeContainer;
    _costConfig = costConfig;

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
    var def = slot.GetItemInstance().Data;
    var amount = def.MaxStackSize > 1 ? $"X{slot.Amount}/{def.MaxStackSize}" : $"X{slot.Amount}";
    return new TooltipData(def.Name, BuildRoleDesc(def, amount));
  }

  private string BuildRoleDesc(IItemDefinition def, string amount)
  {
    switch (def.Role)
    {
      case EItemRole.SkillCaster: return BuildSkillCasterDesc(def, amount);
      case EItemRole.Tool: return BuildToolDesc(def, amount);
      case EItemRole.Placeable: return BuildPlaceableDesc(def, amount);
      case EItemRole.Consumable: return $"Size  {amount}";
      default: return $"{def.Role}  {amount}";
    }
  }

  private string BuildSkillCasterDesc(IItemDefinition def, string amount)
  {
    var desc = $"StackSize  {amount}";
    if (def.Skill == null) return desc;

    var statService = new ItemStatService(def, _statDatabase, _upgradeContainer);
    var damage = statService.GetStat(_statDatabase.Damage);
    var radius = statService.GetStat(_statDatabase.Radius);
    var range = def.Skill.GetBaseStat(_statDatabase.Range);
    var cooldown = statService.GetStat(_statDatabase.Cooldown);
    if (damage > 0) desc += $"\nDamage: {damage:0.#}";
    if (radius  > 0) desc += $"\nRadius: {radius:0.#}";
    if (range  > 0) desc += $"\nRange: {range:0.#}";
    if (cooldown > 0) desc += $"\nCooldown: {cooldown:0.#}s";

    return desc;
  }

  private string BuildToolDesc(IItemDefinition def, string amount)
  {
    var lines = new List<string>();

    var p = def.InteractionProfile;
    if (p?.SupportedIntents != null)
    {
      foreach (var intent in p.SupportedIntents)
      {
        if (_costConfig.TryGetIntentCost(intent, def, ETargetType.All, out var cost))
        {
          var costParts = new List<string>();
          if (cost.EnergyCost > 0) costParts.Add($"{cost.EnergyCost} Energy");
          if (costParts.Count > 0)
            lines.Add($"{intent}: {string.Join(", ", costParts)}");
        }
      }
    }

    if (def.Skill != null)
    {
      var statService = new ItemStatService(def, _statDatabase, _upgradeContainer);
      var damage = statService.GetStat(_statDatabase.Damage);
      var radius = statService.GetStat(_statDatabase.Radius);
      var range = def.Skill.GetBaseStat(_statDatabase.Range);
      var cooldown = statService.GetStat(_statDatabase.Cooldown);
      if (damage > 0) lines.Add($"Damage: {damage:0.#}");
      if (radius > 0) lines.Add($"Radius: {radius:0.#}");
      if (range > 0) lines.Add($"Range: {range:0.#}");
      if (cooldown > 0) lines.Add($"Cooldown: {cooldown:0.#}s");
    }

    return string.Join("\n", lines);
  }

  private string BuildPlaceableDesc(IItemDefinition def, string amount)
  {
    var desc = $"StackSize  {amount}";
    var p = def.PlacementProfile;
    if (p != null && p.GridSize != Vector2Int.zero)
      desc += $"\nSize: {p.GridSize.x}x{p.GridSize.y}";
    return desc;
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