public interface IGameState
{
    EGameState Type { get; }
    void Enter();
    void Exit();
    void Tick();
    void FixedTick();
}