
using System;
using UnityEngine;

public class InputManager 
{
    public InputMode CurrentMode { get; private set; } = InputMode.Gameplay;
    public event Action<InputMode> OnInputModeChanged;

    public void SetInputMode(InputMode newMode)
    {
        if (CurrentMode == newMode) return;
        CurrentMode = newMode;
        OnInputModeChanged?.Invoke(newMode);
        Debug.Log($"Input mode switched to: {newMode}");
    }
}
