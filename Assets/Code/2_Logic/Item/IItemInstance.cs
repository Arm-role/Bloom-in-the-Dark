public interface IItemInstance
{
    IItemDefinition Data { get; }
    int Level { get; }

    float GetMultiplier(StatKey key);
    float GetFlatBonus(StatKey key);
}