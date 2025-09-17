using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemActionController : MonoBehaviour
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private IDrag _currentState;
    private Vector2 _startDragPosition;

    private InteractionService _interactionService;
    private PlayerInventory _playerInventory;

    private ParticalService _particalService;


    #region dragState

    [Header("Drag Settings")]
    [SerializeField] private float holdThreshold = 0.5f;
    [SerializeField] private float holdMoveTolerance = 0.5f;

    private float _holdTimer = 0f;
    private bool _hasMovedTooMuch = false;

    public Vector2 lastTouchPos { get; private set; }

    #endregion
    private void Start()
    {
        SetState(new Idle_DragState());
    }

    public void Initialze(
        InteractionService interactionService,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _interactionService = interactionService;
        _playerInventory = playerInventory;

        _particalService = particalService;
    }

    public void ManualUpdate(IPlayerInput playerInput)
    {
        if (_currentState == null) return;

        Vector3 mouseWorldPos = playerInput.PointerWorldPosition;
        Vector2 pointerWorldPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        var context = new DragContext(
         sourceItem: GetItemOnSlot(),
         startPosition: _startDragPosition,
         currentPosition: pointerWorldPosition,
         moveTolerance: holdMoveTolerance,
         holdThresholdTime: holdThreshold,
         deltaTime: Time.deltaTime,
         elapsedHoldTime: _holdTimer,
         exceededMoveTolerance: _hasMovedTooMuch,
         isPrimaryAction: playerInput.IsPrimaryActionDown,
         isSecondaryAction: playerInput.IsSecorndaryActionDown,
         isReleased: playerInput.IsPrimaryActionReleased,
         hitColliders: Physics2D.OverlapPointAll(pointerWorldPosition)
     );

        lastTouchPos = pointerWorldPosition;

        StateExecutionResult result = _currentState.OnExecute(context);
        ProcessStateResult(result);
    }

    private void SetState(IDrag newState)
    {
        if (_currentState != null)
        {
            var exitResult = _currentState.OnExit();
            ProcessInteractionResult(exitResult, _itemInstance);
        }

        _currentState = newState;

        if (_currentState != null)
        {
            var enterResult = _currentState.OnEnter();
            ProcessInteractionResult(enterResult, _itemInstance);
        }
    }
    private void ProcessStateResult(StateExecutionResult result)
    {
        if (result == null) return;

        if (result.InteractionResult != null)
        {
            ProcessInteractionResult(result.InteractionResult, _itemInstance);
        }
        if (result.NextState != null)
        {
            SetState(result.NextState);
        }
    }
    private void ProcessInteractionResult(InteractionResult result, IItemInstance itemInstance)
    {
        if (result == null) return;

        if (result.StateUpdate != null)
        {
            if (result.StateUpdate.NewHoldTimer.HasValue)
                _holdTimer = result.StateUpdate.NewHoldTimer.Value;
            if (result.StateUpdate.NewHasMovedTooMuch.HasValue)
                _hasMovedTooMuch = result.StateUpdate.NewHasMovedTooMuch.Value;
        }

        if (result.ShouldClearItem) ClearItem();
        if (result.SourceItem != null) SetItem(result.SourceItem);

        if (result.TargetCollider != null)
        {
            var targetcoll = result.TargetCollider;
            IDrop drop = _interactionService.GetDropResolve(_itemInstance.ItemData.Type, targetcoll);

            if (drop == null) return;
            var dropResult = drop.Execute(_itemInstance);

            ProcessDropResult(dropResult, _itemInstance, targetcoll);
        }

        if (result.IsPrimaryAction)
        {
            IPrimaryAction action = _interactionService.GetPrimaryActionResolve(_itemInstance.ItemData.Type, _itemInstance.ItemData.Name);

            if (action == null) return;
            var actionResult = action.Execute(_itemInstance);

            ProcessPrimaryResult(actionResult, _itemInstance);
        }
    }




    private async void ProcessPrimaryResult(PrimaryActionExecutionResult result, IItemInstance sourceItem)
    {
        if (result == null) return;

        if (result.SourceItemInstance != null)
        {
            result.SourceItemInstance.Invoke(sourceItem);
        }

        if (result.ParticleToPlay != null)
        {
            Debug.Log($"Player Particle {result.ParticleToPlay}");
            _particalService.Play(result.ParticleToPlay, lastTouchPos);
        }

        if (await result.ShouldSpawnSelf)
        {
            Debug.Log($"SpawnObject {sourceItem.ItemData.Name}");
        }
    }
    private async void ProcessDropResult(DropExecutionResult result, IItemInstance sourceItem, Collider2D targetCollider)
    {
        if (result == null) return;

        if (result.TargetInteraction != null)
        {
            result.TargetInteraction.Invoke(targetCollider);
        }

        if (result.SourceItemInstance != null)
        {
            result.SourceItemInstance.Invoke(sourceItem);
        }

        if (result.ParticleToPlay != null)
        {
            Debug.Log($"Player Particle {result.ParticleToPlay}");
        }

        if (await result.ShouldDestroyTarget)
        {
            if (targetCollider.TryGetComponent<IInteractable>(out var targetObject))
            {
                targetObject.RequestDestruction();
            }
        }
    }

    private IItemInstance GetItemOnSlot()
    {
        var slot = _playerInventory.GetHotbarSlotSelected();
        if (slot.IsEmpty) return null;

        return slot.Item;
    }
    private void SetItem(IItemInstance itemInstance) => _itemInstance = itemInstance;
    private void ClearItem() => _itemInstance = null;

    internal void StartPrimaryAction()
    {
        throw new NotImplementedException();
    }
}
