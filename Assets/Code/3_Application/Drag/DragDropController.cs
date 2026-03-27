using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropController : IDragDropController
{
  private IPlayerInput _playerInput;

  public InputActionType CurrentHeldActions { get; private set; }

  public event Action<InteractionContext> OnInteraction;

  private readonly List<IHoverResolver> _hoverResolvers = new();
  public HoverState CurrentHoverState { get; private set; }

  private readonly Dictionary<InputActionType, IHoldGestureResolver> _holdResolvers
      = new();

  public DragDropController(
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

  public void RegisterHoverResolver(IHoverResolver resolver)
  {
    _hoverResolvers.Add(resolver);
  }

  public void ManualUpdate()
  {
    var snap = ReadSnapshot();

    UpdateHover(Input.mousePosition);

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

    if (CurrentHoverState == HoverState.UI)
      return;

    OnInteraction?.Invoke(new InteractionContext(
        pressed: snap.Pressed,
        held: resolvedHeld,
        released: snap.Released,
        lastPointerPosition: snap.PointerPosition,
        useSourceItem: true));
  }

  private void UpdateHover(Vector2 screenPosition)
  {
    HoverState state = HoverState.None;

    foreach (var resolver in _hoverResolvers)
      state |= resolver.Resolve(screenPosition);

    //Debug.Log(state.ToString());
    CurrentHoverState = state;
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
