using UnityEngine;

public class MainInventoryController : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    private IInventoryView _mainView;
    private IPlayerInput _input;

    public void Initialize(PlayerInventory playerInventory, IInventoryView view, IPlayerInput input)
    {
        _playerInventory = playerInventory;
        _mainView = view;
        _input = input;

        _mainView.OnSlotClicked += OnSlotClicked;

        _input.OnInventoryToggle += ToggleInventory;
    }

    private void OnDestroy()
    {
        if (_mainView != null)
            _mainView.OnSlotClicked -= OnSlotClicked;

        if (_input != null)
            _input.OnInventoryToggle -= ToggleInventory;
    }

    private void ToggleInventory()
    {
        bool state = !_mainView.Root.activeSelf;
        _mainView.Root.SetActive(state);

        if (state)
            _mainView.UnselectAll();
    }

    private void OnSlotClicked(int index)
    {
        int hotbarSlot = _playerInventory.Hotbar.Slots.FindIndex(s => s.IsEmpty);

        if (hotbarSlot != -1)
            _playerInventory.MoveFromInventoryToHotbar(index, hotbarSlot);
    }
    public void Enter()
    {
        _mainView.Root.SetActive(true);
        _mainView.UnselectAll();
    }

    public void Exit()
    {
        _mainView.Root.SetActive(false);
    }
}