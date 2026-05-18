using System.Collections.Generic;

public sealed class InventoryCooldownController : IGameSystem
{
  private readonly IInventoryView _hotbarView;
  private readonly IInventoryView _mainView;
  private readonly InventoryService _service;
  private readonly CooldownContainer _cooldowns;

  public InventoryCooldownController(
      IInventoryView hotbarView,
      IInventoryView mainView,
      InventoryService service,
      CooldownContainer cooldowns)
  {
    _hotbarView = hotbarView;
    _mainView = mainView;
    _service = service;
    _cooldowns = cooldowns;
    _cooldowns.OnCooldownEnded += HideAllCooldownForKey;
  }

  public void Enter() { }
  public void Exit() { }
  public void FixedUpdate(float dt) { }
  public void Update(float dt) => Tick();

  public void Tick()
  {
    _cooldowns.Tick();
    foreach (var key in _cooldowns.ActiveKeys)
      UpdateAllForKey(key);
  }

  private void UpdateAllForKey(string key)
  {
    UpdateViewForKey(_hotbarView, _service.GetHotbarSlots(), key);
    UpdateViewForKey(_mainView, _service.GetMainSlots(), key);
  }

  private void UpdateViewForKey(IInventoryView view, IReadOnlyList<InventorySlot> slots, string key)
  {
    for (int i = 0; i < slots.Count; i++)
    {
      var slot = slots[i];
      if (slot.IsEmpty || slot.DisplayName != key) continue;
      if (_cooldowns.TryGetCooldown(slot.DisplayName, out var cd))
        view.ShowCooldown(i, cd.Remaining, cd.Normalized);
    }
  }

  private void HideAllCooldownForKey(string key)
  {
    HideViewCooldownForKey(_hotbarView, _service.GetHotbarSlots(), key);
    HideViewCooldownForKey(_mainView, _service.GetMainSlots(), key);
  }

  private void HideViewCooldownForKey(IInventoryView view, IReadOnlyList<InventorySlot> slots, string key)
  {
    for (int i = 0; i < slots.Count; i++)
    {
      if (!slots[i].IsEmpty && slots[i].DisplayName == key)
        view.HideCooldown(i);
    }
  }
}
