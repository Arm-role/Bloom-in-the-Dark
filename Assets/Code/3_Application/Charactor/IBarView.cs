public interface IBarView
{
    string Name { get; }
    void SetHealth(float current, float max);
    void SetHealthImmediate(float current, float max);
}