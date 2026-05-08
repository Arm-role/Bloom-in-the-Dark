using System;

public interface IPlayerInteractor
{
    event Action<ResourceChangedEvent> OnEnergyChanged;
    void SetMaxEnegy(float amount);
    void SetMaxHealth(float amount);
}
