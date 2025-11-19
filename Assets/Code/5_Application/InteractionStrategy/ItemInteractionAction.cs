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

    private readonly PlayerInventory _playerInventory;
    private readonly ParticalService _particalService;

    private ItemStrategyBundle _globalStrategyBundle;
    private ItemStrategyBundle _localStrategyBundle;

    private Vector2 _lastPointerPosition;

    public ItemInteractionAction(
        InteractionHandleService interactionHandleService,
        InteractionService interactionService,
        Transform playerTransform,
        PlayerData playerData,
        IDragDropController dragDropController,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _interactionHandleService = interactionHandleService;
        _interactionService = interactionService;

        _playerTransform = playerTransform;
        _playerData = playerData;
        _dragDropController = dragDropController;
        _playerInventory = playerInventory;

        _particalService = particalService;

        _dragDropController.OnRequestDisable += Dispose;
        _dragDropController.OnInteraction += ProcessInteractionContext;

        _globalStrategyBundle = _interactionHandleService.GetGlobal();
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

        if(result.LastPointerPosition.HasValue)
        {
            _lastPointerPosition = result.LastPointerPosition.Value;
            _localStrategyBundle?.TargetDetectorPreview?.UpdatePreview(new InteractionHandleContext(
                playerPosition: _playerTransform.position,
                pointerPosition: _lastPointerPosition,
                playerDirection: _playerData.Direction));
        }

        if (result.ActiveActions == InputActionType.Primary)
        {
            if (_itemInstance == null) return;

            IItemBehavior action = _interactionService.GetItemBehaviorResolve(
            _itemInstance.Data.Type,
            _itemInstance.Data.Name);

            if (action == null)
            {
                Debug.Log("action Null");
                return;
            }

            var context = new InteractionHandleContext(
                _itemInstance,
                _playerTransform.position,
                _lastPointerPosition,
                _playerData.Direction,
                result.ActiveActions);

            var actionResult = action.ActionExecute(context);
            ProcessClickResult(actionResult, _itemInstance, auxiliaryInput, _localStrategyBundle);
        }
        else if (result.ActiveActions == InputActionType.Secondary)
        {
            Debug.Log("Secondary");

            var context = new InteractionHandleContext(
                _itemInstance,
                _playerTransform.position,
                _lastPointerPosition,
                _playerData.Direction,
                result.ActiveActions);

            var localAction = new InteractionTargetAction();
            var actionResult = localAction.ActionExecute(context);
            ProcessClickResult(actionResult, _itemInstance, auxiliaryInput, _globalStrategyBundle);
        }

        if (auxiliaryInput.ActiveActions == InputActionType.SkillModifierHeld)
        {
            _localStrategyBundle?.SkillIndicatorPreview?.EnablePreview(new InteractionHandleContext(
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerDirection: _playerData.Direction)); ;
        }

        if (auxiliaryInput.ExecuteActions == InputActionType.SkillModifierHeld)
        {
            _localStrategyBundle?.SkillIndicatorPreview?.UpdatePreview(new InteractionHandleContext(
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerDirection: _playerData.Direction));
        }

        if (auxiliaryInput.ReleasedActions == InputActionType.SkillModifierHeld)
        {
            _localStrategyBundle?.SkillIndicatorPreview?.DisablePreview();
        }
    }

    private async void ProcessClickResult(
        ActionExecutionResult result,
        IItemInstance sourceItem, 
        AuxiliaryInput auxiliaryInput,
        ItemStrategyBundle itemStrategy)
    {
        if (result == null) return;

        if (result.ModifierInput != null)
        {
            result.ModifierInput.Invoke(auxiliaryInput);
        }

        if (result.TargetDetector != null)
        {
            result.TargetDetector.Invoke(itemStrategy.Detector);
        }

        if (result.TargetValidator != null)
        {
            result.TargetValidator.Invoke(itemStrategy.Validator);
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
            result.ActionPerformer.Invoke(itemStrategy.Action);
        }

        if (result.ParticleToPlay != null)
        {
            _particalService.Play(result.ParticleToPlay, _lastPointerPosition);
        }

        if (await result.ShouldSpawnSelf)
        {
            Debug.Log($"SpawnObject {sourceItem.Data.Name}");
        }
    }

    private void SetBundle(ItemStrategyBundle strategy)
    {
        _localStrategyBundle?.TargetDetectorPreview?.DisablePreview();    
        _localStrategyBundle?.SkillIndicatorPreview?.DisablePreview();

        if (strategy == null) return;

        _localStrategyBundle = strategy;

        _localStrategyBundle.Action?.Setup();
        _localStrategyBundle.Detector?.Setup(new InteractionHandleContext(
            itemInstance: _itemInstance,
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
           playerDirection: _playerData.Direction));

        _localStrategyBundle.TargetDetectorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));
        _localStrategyBundle.SkillIndicatorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));

        _localStrategyBundle.TargetDetectorPreview?.EnablePreview(new InteractionHandleContext(
            itemInstance: _itemInstance,
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerDirection: _playerData.Direction));
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
            var bundle = _interactionHandleService.Resolve(
                _itemInstance.Data.Type,
                _itemInstance.Data.StategyType);
            SetBundle(bundle);
        }
        else
        {
            _localStrategyBundle?.TargetDetectorPreview?.DisablePreview();
            _localStrategyBundle?.SkillIndicatorPreview?.DisablePreview();
        }
    }
}