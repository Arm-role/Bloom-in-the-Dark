public class PlayerInteractor : IPlayerCommandExecutor
{
    private readonly PlayerEnergy energy;
    private readonly HealthResource health;
    private readonly PlayerInventory inventory;

    public PlayerInteractor(PlayerEnergy energy, HealthResource health, PlayerInventory inventory)
    {
        this.energy = energy;
        this.health = health;   
        this.inventory = inventory;
    }

    public bool CanExecute(IPlayerCommand command)
    {
        switch (command)
        {
            case ConsumeEnergyCommand energyCmd:
                return energy.CanRemove(energyCmd.Amount);

            case ConsumeItemCommand itemCmd:
                return inventory.CanRemoveItem(itemCmd.ItemData, itemCmd.Amount);
            
            case TakeDamageCommand dmg:
                return health.IsAlive;
        }

        return false;
    }
    public bool TryExecute(IPlayerCommand command)
    {
        switch (command)
        {
            case ConsumeEnergyCommand energyCmd:
                return TryConsumeEnergy(energyCmd.Amount);

            case IncreaseMaxEnergyCommand maxEnergyCmd:
                AddMaxEnergy(maxEnergyCmd.Amount);
                return true;
            case ConsumeItemCommand itemCmd:
                return TryUseItem(itemCmd.ItemData, itemCmd.Amount);
            
            case TakeDamageCommand dmg:
                return ApplyDamage(dmg);
        }

        return false;
    }

    private bool TryConsumeEnergy(float amount)
    {
        if (!energy.CanRemove(amount))
            return false;

        energy.Remove(amount);
        return true;
    }

    public void AddMaxEnergy(float ammount)
    {
        energy.AddMax(ammount);
    }

    public void EnergyFill()
    {
        energy.Fill();
    }
    
    private bool TryUseItem(IItemData itemData, int amount)
    {
        if (!inventory.CanRemoveItem(itemData, amount))
            return false;

        inventory.TryRemoveItem(itemData, amount);

        return true;
    }
    
    private bool ApplyDamage(TakeDamageCommand cmd)
    {
        if (!health.IsAlive)
            return false;

        health.TakeDamage(cmd.Amount);
        return true;
    }
    
    public InventorySlot GetSelectedSlot()
        => inventory.GetHotbarSlotSelected();
}