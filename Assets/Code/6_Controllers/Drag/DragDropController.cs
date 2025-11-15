using System;
using UnityEngine;

public class DragDropController : MonoBehaviour, IDragDropController
{
    private IDrag _currentState;
    private Vector2 _startDragPosition;

    private float _holdThreshold = 0.5f;
    private float _holdMoveTolerance = 0.5f;

    private float _holdTimer = 0f;
    private bool _hasMovedTooMuch = false;

    public event Action OnRequestDisable;
    public event Action<InteractionContext, AuxiliaryInput> OnInteraction;

    private void Start()
    {
        SetState(new Idle_DragState());
    }

    private void OnDisable()
    {
        OnRequestDisable?.Invoke();
    }

    public void Initialze(float holdThreshold, float holdMoveTolerance)
    {
        _holdThreshold = holdThreshold;
        _holdMoveTolerance = holdMoveTolerance;
    }

    public void ManualUpdate(IPlayerInput playerInput)
    {
        if (_currentState == null) return;

        Vector3 mouseWorldPos = playerInput.PointerWorldPosition;
        Vector2 pointerWorldPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        var context = new DragContext(
         useSourceItem: true,
         startPosition: _startDragPosition,
         currentPosition: pointerWorldPosition,
         moveTolerance: _holdMoveTolerance,
         holdThresholdTime: _holdThreshold,
         deltaTime: Time.deltaTime,
         elapsedHoldTime: _holdTimer,
         exceededMoveTolerance: _hasMovedTooMuch,
         activeActions: ActiveAction(playerInput),
         releasedActions: ReleasedAction(playerInput)
     );
        StateExecutionResult result = _currentState.OnExecute(context);
        ProcessStateResult(result, playerInput);
    }

    private void SetState(IDrag newState, IPlayerInput playerInput = null)
    {
        if (_currentState != null)
        {
            var exitResult = _currentState.OnExit();
            ProcessInteractionResult(exitResult, playerInput);
        }

        _currentState = newState;

        if (_currentState != null)
        {
            var enterResult = _currentState.OnEnter();
            ProcessInteractionResult(enterResult, playerInput);
        }
    }
    private void ProcessStateResult(StateExecutionResult result, IPlayerInput playerInput)
    {
        if (result == null) return;

        if (result.InteractionResult != null)
        {
            ProcessInteractionResult(result.InteractionResult, playerInput);
        }
        if (result.NextState != null)
        {
            SetState(result.NextState, playerInput);
        }
    }
    private void ProcessInteractionResult(InteractionResult result, IPlayerInput playerInput)
    {
        if (result == null) return;

        if (result.StateUpdate != null)
        {
            if (result.StateUpdate.NewHoldTimer.HasValue)
                _holdTimer = result.StateUpdate.NewHoldTimer.Value;
            if (result.StateUpdate.NewHasMovedTooMuch.HasValue)
                _hasMovedTooMuch = result.StateUpdate.NewHasMovedTooMuch.Value;
        }

        OnInteraction?.Invoke(result.Context, ReadAuxiliaryInput(playerInput));
    }

    private InputActionType ActiveAction(IPlayerInput playerInput)
    {
        InputActionType dragAction = InputActionType.None;

        if (playerInput.IsPrimaryActionDown)
            dragAction |= InputActionType.Primary;

        if (playerInput.IsSecondaryActionDown)
            dragAction |= InputActionType.Secondary;

        return dragAction;
    }

    private InputActionType ExecuteAction(IPlayerInput playerInput)
    {
        InputActionType dragAction = InputActionType.None;

        if (playerInput.IsPrimaryActionPressed)
            dragAction |= InputActionType.Primary;

        if (playerInput.IsSecondaryActionPressed)
            dragAction |= InputActionType.Secondary;

        return dragAction;
    }

    private InputActionType ReleasedAction(IPlayerInput playerInput)
    {
        InputActionType dragAction = InputActionType.None;

        if (playerInput.IsPrimaryActionReleased)
            dragAction |= InputActionType.Primary;

        if (playerInput.IsSecondaryActionReleased)
            dragAction |= InputActionType.Secondary;
        return dragAction;
    }
    private AuxiliaryInput ReadAuxiliaryInput(IPlayerInput playerInput)
    {
        InputActionType activeAction = InputActionType.None;
        InputActionType executeAction = InputActionType.None;
        InputActionType releasedAction = InputActionType.None;


        if (playerInput.IsSkillModifierHeldDown)
            activeAction |= InputActionType.SkillModifierHeld;

        if (playerInput.IsSkillModifierHeld)
            executeAction |= InputActionType.SkillModifierHeld;

        if (playerInput.IsSkillModifierHeldUp)
            releasedAction |= InputActionType.SkillModifierHeld;

        return new AuxiliaryInput(activeAction, executeAction, releasedAction);
    }
}
