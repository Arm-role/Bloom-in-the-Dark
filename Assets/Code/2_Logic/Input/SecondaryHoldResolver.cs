using UnityEngine;

public class SecondaryHoldResolver : IHoldGestureResolver
{
    private readonly float _holdThreshold;
    private readonly float _dragTolerance;

    private float _timer;
    private Vector2 _startPos;
    private bool _tracking;

    public SecondaryHoldResolver(float holdThreshold, float dragTolerance)
    {
        _holdThreshold = holdThreshold;
        _dragTolerance = dragTolerance;
    }

    public InputActionType Resolve(InputSnapshot snap, float deltaTime)
    {
        bool pressed = snap.Pressed.HasFlag(InputActionType.Secondary);
        bool held = snap.Held.HasFlag(InputActionType.Secondary);
        bool released = snap.Released.HasFlag(InputActionType.Secondary);

        if (pressed)
        {
            _tracking = true;
            _timer = 0f;
            _startPos = snap.PointerPosition;
        }

        if (!_tracking)
            return InputActionType.None;

        if (released)
        {
            Reset();
            return InputActionType.None;
        }

        if (held)
        {
            _timer += deltaTime;

            if (_timer >= _holdThreshold)
                return InputActionType.Secondary;

            if (Vector2.Distance(_startPos, snap.PointerPosition) >= _dragTolerance)
                return InputActionType.Secondary;
        }

        return InputActionType.None;
    }

    public void Reset()
    {
        _tracking = false;
        _timer = 0f;
    }
}