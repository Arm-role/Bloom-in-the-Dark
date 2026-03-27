public class PlayerInteractor : IPlayerCommandExecutor
{
  private readonly PlayerEnergy _energy;
  private readonly HealthResource _health;
  private readonly PlayerInventory _inventory;
  private readonly PlayerActionLock _actionLock;
  private readonly CooldownContainer _cooldowns;

  public CooldownContainer CooldownContainer => _cooldowns;

  public PlayerInteractor(
    PlayerEnergy energy,
    HealthResource health,
    PlayerInventory inventory,
    PlayerActionLock actionLock,
    CooldownContainer cooldowns)
  {
    _energy = energy;
    _health = health;
    _inventory = inventory;
    _actionLock = actionLock;
    _cooldowns = cooldowns;
  }

  public bool CanExecute(IPlayerCommand command)
  {
    switch (command)
    {
      case ConsumeEnergyCommand energyCmd:
        return _energy.CanRemove(energyCmd.Amount);

      case ConsumeItemCommand itemCmd:
        return _inventory.CanRemoveItem(itemCmd.ItemData, itemCmd.Amount);

      case TakeDamageCommand dmg:
        return _health.IsAlive;

      case PlayerActionCooldownCommand cool:
        return IsOnCooldown(cool);
    }

    return false;
  }

  public bool TryExecute(IPlayerCommand command)
  {
    switch (command)
    {
      case ConsumeEnergyCommand energyCmd:
        return TryConsumeEnergy(energyCmd.Amount);

      case ConsumeItemCommand itemCmd:
        return TryUseItem(itemCmd.ItemData, itemCmd.Amount);

      case IncreaseMaxEnergyCommand maxEnergyCmd:
        AddMaxEnergy(maxEnergyCmd.Amount);
        return true;

      case TakeDamageCommand dmg:
        return ApplyDamage(dmg);

      case PlayerActionCooldownCommand cool:
        return TryApplyCooldow(cool);
    }

    return false;
  }

  // --------------------------
  // Energy
  // --------------------------

  private bool TryConsumeEnergy(float amount)
  {
    if (!_energy.CanRemove(amount))
      return false;

    //Debug.Log("Remove energy");
    _energy.Remove(amount);
    return true;
  }

  public void AddMaxEnergy(float ammount)
  {
    //Debug.Log("Adding max energy");
    _energy.AddMax(ammount);
  }

  // --------------------------
  // Inventory
  // --------------------------

  private bool TryUseItem(IItemDefinition itemData, int amount)
  {
    if (!_inventory.CanRemoveItem(itemData, amount))
      return false;

    _inventory.TryRemoveItem(itemData, amount);

    return true;
  }

  public InventorySlot GetSelectedSlot()
    => _inventory.GetHotbarSlotSelected();

  public IItemInstance GetEmptyItem()
    => _inventory.GetEmptyItem();

  // --------------------------
  // Damage
  // --------------------------

  private bool ApplyDamage(TakeDamageCommand cmd)
  {
    if (!_health.IsAlive)
      return false;

    _health.TakeDamage(cmd.Amount);
    return true;
  }

  // --------------------------
  // ActionLock
  // --------------------------

  public bool IsBusy() => _actionLock.IsBusy;
  public bool TryStartAction(string actionKey, float duration)
  {
    return _actionLock.TryLock(actionKey, duration);
  }

  // --------------------------
  // Cooldown
  // --------------------------

  private bool IsOnCooldown(PlayerActionCooldownCommand cool)
  {
    return !_cooldowns.IsOnCooldown(cool.CooldownGroup);
  }

  private bool TryApplyCooldow(PlayerActionCooldownCommand cool)
  {
    if (!_cooldowns.TryApply(
            cool.CooldownGroup,
            cool.CooldownDuration))
    {
      return false;
    }
    return true;
  }
}