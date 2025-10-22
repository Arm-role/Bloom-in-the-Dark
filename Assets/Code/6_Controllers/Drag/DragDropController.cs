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
    public event Action<InteractionContext> OnInteraction;

    private void Start()
    {
        SetState(new Idle_DragState());
    }

    private void OnDisable()
    {
        OnRequestDisable?.Invoke();
    }

    public void Initialze(float holdThreshold,float holdMoveTolerance)
    {
        _holdThreshold = holdThreshold;
        _holdMoveTolerance = holdMoveTolerance;
    }

    public void ManualUpdate(IPlayerInput playerInput, Collider2D[] hits)
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
         isPrimaryAction: playerInput.IsPrimaryActionDown,
         isPrimaryActionReleased: playerInput.IsPrimaryActionReleased,
         isSecondaryAction: playerInput.IsSecondaryActionDown,
         isSecondaryActionReleased: playerInput.IsSecondaryActionReleased,
         hitColliders: hits
     );
        StateExecutionResult result = _currentState.OnExecute(context);
        ProcessStateResult(result);
    }

    private void SetState(IDrag newState)
    {
        if (_currentState != null)
        {
            var exitResult = _currentState.OnExit();
            ProcessInteractionResult(exitResult);
        }

        _currentState = newState;

        if (_currentState != null)
        {
            var enterResult = _currentState.OnEnter();
            ProcessInteractionResult(enterResult);
        }
    }
    private void ProcessStateResult(StateExecutionResult result)
    {
        if (result == null) return;

        if (result.InteractionResult != null)
        {
            ProcessInteractionResult(result.InteractionResult);
        }
        if (result.NextState != null)
        {
            SetState(result.NextState);
        }
    }
    private void ProcessInteractionResult(InteractionResult result)
    {
        if (result == null) return;

        if (result.StateUpdate != null)
        {
            if (result.StateUpdate.NewHoldTimer.HasValue)
                _holdTimer = result.StateUpdate.NewHoldTimer.Value;
            if (result.StateUpdate.NewHasMovedTooMuch.HasValue)
                _hasMovedTooMuch = result.StateUpdate.NewHasMovedTooMuch.Value;
        }
        OnInteraction?.Invoke(result.Context);
    }
}