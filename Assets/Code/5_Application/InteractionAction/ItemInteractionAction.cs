using System;
using UnityEngine;

public class ItemInteractionAction
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private readonly Transform _playerTransform;
    private readonly PlayerData _playerData;
    private readonly IDragDropController _dragDropController;
    private readonly InteractionHandleService _interactionHandleService;
    private readonly InteractionService _interactionService;
    private readonly InteractionTargetResolver _targetResolver;

    private readonly PlayerInventory _playerInventory;
    private readonly ParticalService _particalService;

    private ITargetDetector _currentDetector;
    private ITargetDetectorPreview _targetDetectorPreview;
    private ISkillIndicatorPreview _skillIndicatorPreview;
    private ITargetValidator _currentValidator;
    private IActionPerformer _currentItemAction;

    private Vector2 _lastPointerPosition;

    public ItemInteractionAction(
        InteractionHandleService interactionHandleService,
        InteractionService interactionService,
        InteractionTargetResolver targetResolver,
        Transform playerTransform,
        PlayerData playerData,
        IDragDropController dragDropController,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _interactionHandleService = interactionHandleService;
        _interactionService = interactionService;
        _targetResolver = targetResolver;

        _playerTransform = playerTransform;
        _playerData = playerData;
        _dragDropController = dragDropController;
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
    private void ProcessInteractionContext(InteractionContext result, AuxiliaryInput auxiliaryInput)
    {
        if (result.UseSourceItem) OnItemChanged(GetItemOnSlot());
        if (_itemInstance == null) return;

        if (result.LastPointerPosition != null)
        {
            _lastPointerPosition = result.LastPointerPosition.Value;
            _targetDetectorPreview?.UpdatePreview(new InteractionHandleContext(
                playerPosition: _playerTransform.position,
                pointerPosition: _lastPointerPosition,
                playerForward: _playerData.MoveDirection));
        }

        IItemBehavior action = _interactionService.GetItemBehaviorResolve(
            _itemInstance.ItemData.Type,
            _itemInstance.ItemData.Name);

        if (action == null)
        {
            Debug.Log("action Null");
            return;
        }

        if (result.ActiveActions == InputActionType.Primary)
        {
            var actionResult = action.ActionExecute(new InteractionHandleContext(
                _itemInstance,
                _playerTransform.position,
                _lastPointerPosition,
                _playerData.MoveDirection));
            ProcessClickResult(actionResult, _itemInstance, auxiliaryInput);
        }

        if (auxiliaryInput.ActiveActions == InputActionType.SkillModifierHeld)
        {
            _skillIndicatorPreview?.EnablePreview(new InteractionHandleContext(
                playerPosition: _playerTransform.position,
                pointerPosition: _lastPointerPosition,
                playerForward: _playerData.MoveDirection));
        }

        if (auxiliaryInput.ExecuteActions == InputActionType.SkillModifierHeld)
        {
            _skillIndicatorPreview?.UpdatePreview(new InteractionHandleContext(
                playerPosition: _playerTransform.position,
                pointerPosition: _lastPointerPosition,
                playerForward: _playerData.MoveDirection));
        }

        if (auxiliaryInput.ReleasedActions == InputActionType.SkillModifierHeld)
        {
            _skillIndicatorPreview?.DisablePreview();
        }
    }

    private async void ProcessClickResult(ActionExecutionResult result, IItemInstance sourceItem, AuxiliaryInput auxiliaryInput)
    {
        if (result == null) return;

        if (result.ModifierInput != null)
        {
            result.ModifierInput.Invoke(auxiliaryInput);
        }

        if (result.TargetDetector != null)
        {
            result.TargetDetector.Invoke(_currentDetector);
        }

        if (result.TargetValidator != null)
        {
            result.TargetValidator.Invoke(_currentValidator);
        }

        if (result.InventoryInteraction != null)
        {
            result.InventoryInteraction.Invoke(_playerInventory.Hotbar);
        }

        if (result.PlayerData != null)
        {
            result.PlayerData.Invoke(_playerData);
        }

        if (result.ActionPerformer != null)
        {
            result.ActionPerformer.Invoke(_currentItemAction);
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
    private void ProcessDropResult(DropExecutionResult result, IItemInstance sourceItem, InteractionTargetContext target)
    {
        if (result == null) return;

        if (result.TargetInteraction != null)
        {
            result.TargetInteraction.Invoke(target);

            if (target.IsObject) Debug.Log(target.Collider.gameObject.name);
        }

        if (result.SourceItemInstance != null)
        {
            result.SourceItemInstance.Invoke(sourceItem);
        }

        if (result.ParticleToPlay != null)
        {
            _particalService.Play(result.ParticleToPlay, _lastPointerPosition);
        }
    }

    private void SetBundle(ItemStrategyBundle strategy)
    {
        _targetDetectorPreview?.DisablePreview();
        _skillIndicatorPreview?.DisablePreview();

        if (strategy == null) return;

        _currentDetector = strategy.Detector;
        _currentValidator = strategy.Validator;
        _targetDetectorPreview = strategy.TargetDetectorPreview;
        _skillIndicatorPreview = strategy.SkillIndicatorPreview;
        _currentItemAction = strategy.Action;

        _currentItemAction?.Setup();
        _currentDetector?.Setup(new InteractionHandleContext(itemInstance: _itemInstance, playerPosition: _playerTransform.position));

        _targetDetectorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));
        _skillIndicatorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));

        _targetDetectorPreview?.EnablePreview(new InteractionHandleContext(
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerForward: _playerData.MoveDirection));
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
            var bundle = _interactionHandleService.Resolve(_itemInstance.ItemData.Type, _itemInstance.ItemData.StategyType);
            SetBundle(bundle);
        }
        else
        {
            _targetDetectorPreview?.DisablePreview();
            _skillIndicatorPreview?.DisablePreview();
        }
    }
}