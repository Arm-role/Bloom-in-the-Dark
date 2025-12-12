    public interface IPlayerEnergy
{
    float GetPercent();

    void SetEnergy(float value);

    void SetMaxEnergy(float value, bool refill = false);

    void Add(float amount);

    void Remove(float amount);
    void AddMax(float amount, bool refill = false);

    void RemoveMax(float amount);
    void Fill();

    bool CanAdd(float amount);
    bool CanRemove(float amount);
}
