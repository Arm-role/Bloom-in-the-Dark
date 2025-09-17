using System;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    private IPlayerInput _playerInput;
    private int _currentSlotIndex = 0;
    private int _totalSlots;

    public Action<int> SlotSelected;

    public void Initialize(IPlayerInput playerInput)
    {
        _playerInput = playerInput;
    }

    public void SetTotalSlots(int totalSlots)
    {
        _totalSlots = totalSlots;
    }

    private void Update()
    {
        if (_playerInput == null) return;

        float scroll = _playerInput.ScrollDelta;

        if (scroll != 0)
        {
            _currentSlotIndex -= (int)scroll;

            if (_currentSlotIndex >= _totalSlots)
            {
                _currentSlotIndex = 0;
            }
            if (_currentSlotIndex < 0)
            {
                _currentSlotIndex = _totalSlots - 1;
            }

            Debug.Log("Switched to Hotbar Slot: " + _currentSlotIndex);
        }
    }
}
