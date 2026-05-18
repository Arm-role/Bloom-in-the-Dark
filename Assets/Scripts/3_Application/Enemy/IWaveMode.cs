public interface IWaveMode
{
    bool IsFinished { get; }
    void Tick(float dt);
}
