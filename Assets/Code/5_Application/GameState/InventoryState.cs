using System;

public class InventoryState : IGameState
{
    public EGameState Type => EGameState.Inventory;

    private Action _onEnter;
    private Action _onExit;

    public void Initialize(Action onEnter, Action onExit)
    {
        _onEnter = onEnter;
        _onExit = onExit;
    }
    public void Enter() => _onEnter?.Invoke();
    public void Exit() => _onExit?.Invoke();

    public void Tick() { }
    public void FixedTick() { }
}