using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropController : MonoBehaviour, IDragDropController
{
    private IPlayerInput _playerInput;

    public InputActionType CurrentHeldActions { get; private set; }

    public event Action OnRequestDisable;
    public event Action<InteractionContext> OnInteraction;

    private readonly Dictionary<InputActionType, IHoldGestureResolver> _holdResolvers
        = new();

    private Vector2 _lastPointerPosition;

    public void Initialize(
        IPlayerInput playerInput,
        float secondaryHoldThreshold,
        float secondaryDragTolerance)
    {
        _playerInput = playerInput;

        // register resolvers
        _holdResolvers[InputActionType.Secondary] =
            new SecondaryHoldResolver(
                secondaryHoldThreshold,
                secondaryDragTolerance);

        _holdResolvers[InputActionType.Primary] =
            new InstantHoldResolver(InputActionType.Primary);
    }

    private void OnDisable()
    {
        OnRequestDisable?.Invoke();
    }

    public void ManualUpdate()
    {
        var snap = ReadSnapshot();

        InputActionType resolvedHeld = InputActionType.None;

        foreach (var resolver in _holdResolvers)
        {
            resolvedHeld |= resolver.Value.Resolve(snap, Time.deltaTime);
        }

        foreach (var pair in _holdResolvers)
        {
            var action = pair.Key;
            var resolver = pair.Value;

            if (snap.Released.HasFlag(action))
                resolver.Reset();
        }

        CurrentHeldActions = resolvedHeld;
        _lastPointerPosition = snap.PointerPosition;

        OnInteraction?.Invoke(new InteractionContext(
            pressed: snap.Pressed,
            held: resolvedHeld,
            released: snap.Released,
            lastPointerPosition: snap.PointerPosition,
            useSourceItem: true));
    }

    private InputSnapshot ReadSnapshot()
    {
        Vector3 worldPos = _playerInput.PointerWorldPosition;

        return new InputSnapshot
        {
            Pressed = ReadPressed(),
            Held = ReadHeld(),
            Released = ReadReleased(),
            PointerPosition = new Vector2(worldPos.x, worldPos.y)
        };
    }

    private InputActionType ReadPressed()
    {
        InputActionType result = InputActionType.None;

        if (_playerInput.IsPrimaryActionPressed)
            result |= InputActionType.Primary;

        if (_playerInput.IsSecondaryActionPressed)
            result |= InputActionType.Secondary;

        return result;
    }

    private InputActionType ReadHeld()
    {
        InputActionType result = InputActionType.None;

        if (_playerInput.IsPrimaryActionHeld)
            result |= InputActionType.Primary;

        if (_playerInput.IsSecondaryActionHeld)
            result |= InputActionType.Secondary;

        return result;
    }

    private InputActionType ReadReleased()
    {
        InputActionType result = InputActionType.None;

        if (_playerInput.IsPrimaryActionReleased)
            result |= InputActionType.Primary;

        if (_playerInput.IsSecondaryActionReleased)
            result |= InputActionType.Secondary;

        return result;
    }
}
