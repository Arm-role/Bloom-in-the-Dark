using System;

public interface IPlayerInteractor
{
    event Action<ResourceChangedEvent> OnEnergyChanged;
    void SetMaxEnergy(float amount);
    void SetMaxHealth(float amount);
}
