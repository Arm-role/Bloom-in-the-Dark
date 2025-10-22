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

            SlotSelected?.Invoke(_currentSlotIndex);

            Debug.Log("Switched to Hotbar Slot: " + _currentSlotIndex);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            _currentSlotIndex--;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
        {
            _currentSlotIndex++;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentSlotIndex = 0;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentSlotIndex = 1;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _currentSlotIndex = 2;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _currentSlotIndex = 3;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _currentSlotIndex = 4;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _currentSlotIndex = 5;
            SlotSelected?.Invoke(_currentSlotIndex);
        }
    }
}
