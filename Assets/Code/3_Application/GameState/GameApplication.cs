using System;

public class GameApplication
{
    private readonly GameStateMachine _stateMachine;
    private bool _inventoryOpen;

    public event Action<bool> OnInventoryStateChanged;

    public GameApplication(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Initialize(IPlayerInput input)
    {
        input.OnInventoryToggle += ToggleInventory;
    }

    public void Start()
    {
        _stateMachine.ChangeState(EGameState.Gameplay);
        OnInventoryStateChanged?.Invoke(false);
    }

    private void ToggleInventory()
    {
        _inventoryOpen = !_inventoryOpen;

        if (_inventoryOpen)
            _stateMachine.ChangeState(EGameState.Inventory);
        else
            _stateMachine.ChangeState(EGameState.Gameplay);

        OnInventoryStateChanged?.Invoke(_inventoryOpen);
    }

    public void Update() => _stateMachine.Tick();
    public void FixedUpdate() => _stateMachine.FixedTick();
}