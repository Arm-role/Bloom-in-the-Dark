using System;
using UnityEngine;

public class ItemInteractionAction
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private readonly Transform _playerTransform;

    private readonly PlayerEnergy _playerEnergy;
    private readonly PlayerState _playerState;

    private readonly IDragDropController _dragDropController;

    private readonly InteractionHandleService _interactionHandleService;
    private readonly InteractionService _interactionService;

    private readonly PlayerInventory _playerInventory;
    private readonly ParticalService _particalService;

    private InputStrategyBinding _inputStrategyBinding;
    private ItemStrategyBundle _globalBundle;
    private ItemStrategyBundle _previewBundle;

    private Vector2 _lastPointerPosition;

    public ItemInteractionAction(
        InteractionHandleService interactionHandleService,
        InteractionService interactionService,
        Transform playerTransform,
        PlayerState playerData,
        PlayerEnergy playerEnergy,
        IDragDropController dragDropController,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _interactionHandleService = interactionHandleService;
        _interactionService = interactionService;

        _playerTransform = playerTransform;
        _playerState = playerData;
        _playerEnergy = playerEnergy;
        _dragDropController = dragDropController;
        _playerInventory = playerInventory;

        _particalService = particalService;

        _dragDropController.OnRequestDisable += Dispose;
        _dragDropController.OnInteraction += ProcessInteractionContext;

        _globalBundle = _interactionHandleService.Resolve(EItemStrategyType.DirectInteract);
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

        if (result.LastPointerPosition.HasValue)
        {
            _lastPointerPosition = result.LastPointerPosition.Value;
            _previewBundle?.TargetDetectorPreview?.UpdatePreview(new InteractionHandleContext(
                playerPosition: _playerTransform.position,
                pointerPosition: _lastPointerPosition,
                playerDirection: _playerState.Direction));
        }

        HandleClick(result.ActiveActions, auxiliaryInput);

        if (auxiliaryInput.ActiveActions == InputActionType.SkillModifierHeld)
        {
            _previewBundle?.SkillIndicatorPreview?.EnablePreview(new InteractionHandleContext(
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerDirection: _playerState.Direction)); ;
        }

        if (auxiliaryInput.ExecuteActions == InputActionType.SkillModifierHeld)
        {   
            _previewBundle?.SkillIndicatorPreview?.UpdatePreview(new InteractionHandleContext(
            playerPosition: _playerTransform.position,
            pointerPosition: _lastPointerPosition,
            playerDirection: _playerState.Direction));
        }

        if (auxiliaryInput.ReleasedActions == InputActionType.SkillModifierHeld)
        {
            _previewBundle?.SkillIndicatorPreview?.DisablePreview();
        }
    }
    private void HandleClick(InputActionType input, AuxiliaryInput aux)
    {
        if (_itemInstance == null)
            return;

        if (input == InputActionType.None) return;

        Debug.Log("Intercation");
        var context = new InteractionHandleContext(
            _itemInstance,
            _playerTransform.position,
            _lastPointerPosition,
            _playerState.Direction,
            input);

        var globalDp = _globalBundle.Detector.IntercationExcute(context);

        var globalValid = globalDp.IsValid
            ? _globalBundle.Validator.Validate(globalDp)
            : ValidationResult.Fail("No Global Target");

        var globalCanExec = globalValid.IsValid &&
        _globalBundle.Action.CanExecute(context, globalDp);

        var strategy = _itemInstance.Data.ResolveStrategy(input);

        if (strategy == EItemStrategyType.None)
        {
            if (!globalCanExec)
            {
                Debug.Log("Global cannot execute.");
                return;
            }

            var global = new InteractionTargetAction();
            var globalResult = global.ActionExecute(context);

            Debug.Log("GlobalStrategy");
            ExecuteAction(_globalBundle, globalResult, aux);
            return;
        }

        var itemBundle = _interactionHandleService.Resolve(strategy);

        itemBundle.Detector.Setup(context);

        var itemDp = itemBundle.Detector.IntercationExcute(context);
        var itemValid = itemDp.IsValid
            ? itemBundle.Validator.Validate(itemDp)
            : ValidationResult.Fail("No Item Target");

        var itemCanExec = itemValid.IsValid &&
        itemBundle.Action.CanExecute(context, itemDp);

        ItemStrategyBundle chosen;

        if (itemCanExec)
        {
            chosen = itemBundle;
        }
        else if (globalCanExec)
        {
            chosen = _globalBundle;
        }
        else
        {
            Debug.Log("No interaction or action can be executed.");
            return;
        }

        var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
        _playerState.Look(dir);

        var localAction = _interactionService.GetItemBehaviorResolve(
            _itemInstance.Data.Type, _itemInstance.Data.Name);

        var actionResult = localAction.ActionExecute(context);

        Debug.Log("LocalStrategy");
        ExecuteAction(chosen, actionResult, aux);
    }

    private void ExecuteAction(
        ItemStrategyBundle bundle,
        ActionExecutionResult result,
        AuxiliaryInput auxiliaryInput)
    {
        if (result == null) return;

        if (result.ModifierInput != null)
        {
            result.ModifierInput.Invoke(auxiliaryInput);
        }

        if (result.TargetDetector != null)
        {
            result.TargetDetector.Invoke(bundle.Detector);
        }

        if (result.TargetValidator != null)
        {
            result.TargetValidator.Invoke(bundle.Validator);
        }

        if (result.PlayerEnergy != null)
        {
            result.PlayerEnergy.Invoke(_playerEnergy);
        }

        if (result.InventoryInteraction != null)
        {
            result.InventoryInteraction.Invoke(_playerInventory.Hotbar);
        }

        if (result.PlayerState != null)
        {
            result.PlayerState.Invoke(_playerState);
        }

        if (result.ActionPerformer != null)
        {
            result.ActionPerformer.Invoke(bundle.Action);
        }

        if (result.ParticleToPlay != null)
        {
            _particalService.Play(result.ParticleToPlay, _lastPointerPosition);
        }
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
            var bindings = _itemInstance.Data.StrategyBindings;

            if (bindings.Count == 1)
            {
                _inputStrategyBinding = bindings[0];
                var bundle = _interactionHandleService.Resolve(_inputStrategyBinding.Strategy);

                _previewBundle = bundle;
            }

            _previewBundle.TargetDetectorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));
            _previewBundle.SkillIndicatorPreview?.Setup(new InteractionHandleContext(itemInstance: _itemInstance));

            _previewBundle?.TargetDetectorPreview?.DisablePreview();
            _previewBundle?.SkillIndicatorPreview?.DisablePreview();

            _globalBundle.Detector.Setup(new InteractionHandleContext(
                    itemInstance: _itemInstance,
                    playerPosition: _playerTransform.position,
                    pointerPosition: _lastPointerPosition,
                    playerDirection: _playerState.Direction));

            if (bindings.Count == 1)
            {
                _previewBundle.TargetDetectorPreview?.EnablePreview(new InteractionHandleContext(
                    itemInstance: _itemInstance,
                    playerPosition: _playerTransform.position,
                    pointerPosition: _lastPointerPosition,
                    playerDirection: _playerState.Direction));

            }

            _previewBundle.Action?.Setup();
        }
        else
        {
            _previewBundle?.TargetDetectorPreview?.DisablePreview();
            _previewBundle?.SkillIndicatorPreview?.DisablePreview();
            _previewBundle = null;
        }
    }
}