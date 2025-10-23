using System;
using UnityEngine;

public class ItemInteractionAction
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private readonly Transform _playerTransform;
    private readonly IDragDropController _dragDropController;
    private readonly InteractionHandleService _interactionHandleService;
    private readonly InteractionService _interactionService;
    private readonly PlayerInventory _playerInventory;
    private readonly ParticalService _particalService;

    private IInteractionHandle _currentIteractionHandle;
    private IItemAction _currentItemAction;

    private Vector2 _lastPointerPosition;

    public ItemInteractionAction(
        InteractionHandleService previewLibrary,
        Transform playerTransform,
        IDragDropController dragDropController,
        InteractionService interactionService,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _interactionHandleService = previewLibrary;

        _playerTransform = playerTransform;
        _dragDropController = dragDropController;
        _interactionService = interactionService;
        _playerInventory = playerInventory;

        _particalService = particalService;

        _dragDropController.OnRequestDisable += Dispose;
        _dragDropController.OnInteraction += ProcessInteractionContext;
    }

    private void Dispose()
    {
        if (_dragDropController != null)
        {
            _dragDropController.OnRequestDisable -= Dispose;
            _dragDropController.OnInteraction -= ProcessInteractionContext;
        }
    }
    private void ProcessInteractionContext(InteractionContext result)
    {
        if (result.UseSourceItem) OnItemChanged(GetItemOnSlot());
        if (_itemInstance == null) return;

        if (result.LastPointerPosition != null)
        {
            _lastPointerPosition = result.LastPointerPosition.Value;
            _currentIteractionHandle?.UpdatePreview(new InteractionHandleContext(playerPosition: _playerTransform.position, pointerPosition: _lastPointerPosition));
        }

        IItemBehavior action = _interactionService.GetItemBehaviorResolve(
            _itemInstance.ItemData.Type,
            _itemInstance.ItemData.Name,
            result.TargetCollider);

        if (action == null)
        {
            Debug.Log("action Null");
            return;
        }

        if (result.TargetCollider != null)
        {
            var targetcoll = result.TargetCollider;
            var dropResult = action.DropExecute(_itemInstance, _playerTransform.position, _lastPointerPosition);
            ProcessDropResult(dropResult, _itemInstance, targetcoll);
        }

        if (result.IsPrimaryAction)
        {
            var actionResult = action.PrimaryActionExecute(_itemInstance, _playerTransform.position, _lastPointerPosition);
            ProcessPrimaryResult(actionResult, _itemInstance);
        }

        if (result.IsSecondaryAction)
        {
        }

    }

    private async void ProcessPrimaryResult(PrimaryActionExecutionResult result, IItemInstance sourceItem)
    {
        if (result == null) return;

        if (result.InteractionHandle != null)
        {
            result.InteractionHandle.Invoke(_currentIteractionHandle);
        }

        if (result.InventoryInteraction != null)
        {
            result.InventoryInteraction.Invoke(_playerInventory.Hotbar);
        }

        if (result.ItemAction != null)
        {
            result.ItemAction.Invoke(_currentItemAction);
        }

        if (result.ParticleToPlay != null)
        {
            _particalService.Play(result.ParticleToPlay, _lastPointerPosition);
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

    private void SetPreview((IInteractionHandle handler, IItemAction action)? strategy)
    {
        if (strategy == null) return;

        (IInteractionHandle handler, IItemAction action) str = strategy.Value;
        _currentIteractionHandle?.DisablePreview();

        _currentItemAction = str.action;
        _currentIteractionHandle = str.handler;

        if (_currentIteractionHandle == null || _currentItemAction == null) return;

        _currentItemAction.Setup();
        _currentIteractionHandle.Setup(new InteractionHandleContext(itemInstance: _itemInstance));
        _currentIteractionHandle.EnablePreview(new InteractionHandleContext(playerPosition: _playerTransform.position, pointerPosition: _lastPointerPosition));
    }
    private IItemInstance GetItemOnSlot()
    {
        var slot = _playerInventory.GetHotbarSlotSelected();
        if (slot.IsEmpty) return null;

        return slot.Item;
    }

    private void OnItemChanged(IItemInstance itemInstance)
    {
        if (itemInstance == _itemInstance) return;

        _itemInstance = itemInstance;

        if (_itemInstance != null)
        {
            var preview = _interactionHandleService.GetHandler(_itemInstance.ItemData.Type, _itemInstance.ItemData.StategyType);
            SetPreview(preview);
        }
        else
        {
            _currentIteractionHandle?.DisablePreview();
        }
    }
}