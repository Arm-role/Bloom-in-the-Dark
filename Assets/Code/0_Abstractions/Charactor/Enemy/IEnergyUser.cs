public interface IEnergyUser
{
    bool TryConsumeEnergy(float amount);
    void SetMaxEnergy(float ammount);
}