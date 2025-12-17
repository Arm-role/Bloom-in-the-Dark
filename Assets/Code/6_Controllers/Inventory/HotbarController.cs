using System;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    private IPlayerInput _input;
    private HotbarState _state;

    public void Initialize(IPlayerInput input, HotbarState state)
    {
        _input = input;
        _state = state;

        _input.OnHotbarSelect += _state.SelectSlot;
    }

    private void Update()
    {
        float scroll = _input.ScrollDelta;
        if (scroll > 0) _state.SelectPreviousSlot();
        else if (scroll < 0) _state.SelectNextSlot();
    }
}
