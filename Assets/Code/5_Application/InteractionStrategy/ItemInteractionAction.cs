using System;
using UnityEngine;

public class ItemInteractionAction
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private readonly InteractionHandleService _interactionHandleService;
    private readonly InteractionCostService _interactionCostService;

    private readonly Transform _playerTransform;
    private readonly PlayerInteractor _interactor;
    private readonly PlayerState _playerState;

    private readonly IDragDropController _dragDropController;
    private readonly ParticalService _particalService;

    private ItemInteractionProfile _itemInteractionProfile;
    private ItemStrategyBundle _globalBundle;
    private ItemStrategyBundle _previewBundle;

    private Vector2 _lastPointerPosition;
    private PlayerCooldownController _playerCooldown;

    public ItemInteractionAction(
        InteractionHandleService interactionHandleService,
        InteractionCostService interactionCostService,
        Transform playerTransform,
        PlayerInteractor interactor,
        PlayerState playerState,
        IDragDropController dragDropController,
        ParticalService particalService)
    {
        _interactionHandleService = interactionHandleService;
        _interactionCostService = interactionCostService;

        _interactor = interactor;
        _playerState = playerState;
        _playerTransform = playerTransform;
        _dragDropController = dragDropController;

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

    private void ProcessInteractionContext(InteractionContext result)
    {
        if (result.UseSourceItem)
            OnItemChanged(GetItemOnSlot());

        if (result.LastPointerPosition.HasValue)
            _lastPointerPosition = result.LastPointerPosition.Value;

        if (_itemInstance == null || _itemInteractionProfile == null)
        {
            HandleGlobalInteraction(result.Pressed, InteractionPhase.Pressed);
            HandleGlobalInteraction(result.Pressed, InteractionPhase.Pressed);
            HandleGlobalInteraction(result.Pressed, InteractionPhase.Pressed);
            return;
        }

        HandlePreview(result.Pressed, InteractionPhase.Pressed);
        HandlePreview(result.Held, InteractionPhase.Held);
        HandlePreview(result.Released, InteractionPhase.Released);

        TickPreview();

        HandleInteraction(result.Pressed, InteractionPhase.Pressed);
        HandleInteraction(result.Held, InteractionPhase.Held);
        HandleInteraction(result.Released, InteractionPhase.Released);
    }

    private void HandlePreview(InputActionType input, InteractionPhase phase)
    {
        if (input == InputActionType.None)
            return;

        var ctx = CreateHandleContext(input, EInteractionIntentType.None);

        foreach (var pr in _itemInteractionProfile.GetPreviewRules(
                     input, phase, ItemSelectionPhase.Selected))
        {
            ApplyPreviewRule(pr, ctx);
        }
    }

    private void TickPreview()
    {
        var ctx = CreateHandleContext(
            InputActionType.None,
            EInteractionIntentType.None);

        foreach (var pr in _itemInteractionProfile.GetPreviewRules(
                     InputActionType.None,
                     InteractionPhase.None,
                     ItemSelectionPhase.Selected))
        {
            ApplyPreviewRule(pr, ctx);
        }
    }

    private void ApplyPreviewRule(
        PreviewRule rule,
        InteractionHandleContext ctx)
    {
        Debug.Log("ApplyPreviewRule");
        if (rule.Action == PreviewAction.Disable)
        {
            DisablePreview();
            return;
        }

        Debug.Log(rule.Action);

        var bundle = _interactionHandleService.Resolve(rule.Strategy);
        if (bundle == null)
            return;

        if (_previewBundle == null || _previewBundle != bundle)
        {
            DisablePreview();
            _previewBundle = bundle;
            _previewBundle?.Preview?.Setup();
        }

        Debug.Log("_previewBundle Set");

        var config = _previewBundle.Targeting.ConfigProvider.Create(ctx);
        var target = _previewBundle.Targeting.Strategy.Resolve(ctx, config);
        _previewBundle.Preview?.Update(target);
    }

    private void HandleInteraction(
        InputActionType input,
        InteractionPhase phase)
    {
        if (input == InputActionType.None)
            return;

        var ctx = new InteractionConditionContext
        {
            Pressed = phase == InteractionPhase.Pressed ? input : InputActionType.None,
            Held = _dragDropController.CurrentHeldActions,
            Released = phase == InteractionPhase.Released ? input : InputActionType.None,
            IsSkillPreviewActive = _previewBundle != null,
            Item = _itemInstance
        };

        if (!_itemInteractionProfile.TryGetInteractionRule(input, phase, ctx, out var rule))
            return;

        ExecuteTargeted(rule, input);
    }

    private void HandleGlobalInteraction(
        InputActionType input,
        InteractionPhase phase)
    {
        if (input == InputActionType.None)
            return;

        var handleContext = CreateHandleContext(input, EInteractionIntentType.None);
        ExecuteTargetedGlobal(handleContext);
    }

    private void ExecuteTargeted(
        InteractionRule rule,
        InputActionType input)
    {
        var ctx = CreateHandleContext(input, rule.IntentType);

        // ---------- EXECUTION ----------

        // Strategy = None → ไม่ใช่ item action
        if (rule.Strategy == EItemStrategyType.None)
        {
            Debug.Log("Enter Global");
            if (rule.Fallback == InteractionFallback.Global)
                ExecuteTargetedGlobal(ctx);

            return;
        }

        var bundle = _interactionHandleService.Resolve(rule.Strategy);
        if (bundle == null)
            return;

        var config = bundle.Targeting.ConfigProvider.Create(ctx);
        var target = bundle.Targeting.Strategy.Resolve(ctx, config);

        if (!target.IsValid)
            return;

        // ---- Targeting ----
        bool isValid =
            target.IsValid &&
            (bundle.Targeting.Validator?.Validate(ctx, target).IsValid ?? true) &&
            bundle.Action.CanExecute(ctx, target);

        Debug.Log("isValid " + isValid);

        if (!isValid)
        {
            if (rule.Fallback == InteractionFallback.Global)
                ExecuteTargetedGlobal(ctx);

            return;
        }

        Debug.Log("Local");
        ExecuteAction(ctx, bundle, target, rule.IntentType);
    }

    private void ExecuteTargetedGlobal(InteractionHandleContext ctx)
    {
        var bundle = _globalBundle;

        var config = bundle.Targeting.ConfigProvider.Create(ctx);
        var target = bundle.Targeting.Strategy.Resolve(ctx, config);

        if (!target.IsValid)
            return;

        Debug.Log("Global");
        ExecuteAction(ctx, bundle, target, EInteractionIntentType.Harvest);
    }

    private async void ExecuteAction(
        InteractionHandleContext ctx,
        ItemStrategyBundle bundle,
        TargetResult targetResult,
        EInteractionIntentType intentType)
    {
        InteractionResult result = await bundle.Action.Execute(ctx, targetResult);

        Debug.Log(result.IsConsumed);

        if (_interactionCostService.TryResolve(
                intentType,
                result,
                out var feedback))
        {
            ApplyFeedback(ctx, feedback);
        }
    }

    private IItemInstance GetItemOnSlot()
    {
        var slot = _interactor.GetSelectedSlot();
        if (slot.IsEmpty) return null;

        return slot.Item;
    }

    private void OnItemChanged(IItemInstance itemInstance)
    {
        if (itemInstance == _itemInstance)
            return;

        DisablePreview();

        _itemInstance = itemInstance;
        _itemInteractionProfile =
            itemInstance?.Data.InteractionProfile as ItemInteractionProfile;

        if (_itemInteractionProfile == null)
            return;

        var ctx = CreateHandleContext(InputActionType.None, EInteractionIntentType.None);

        foreach (var pr in _itemInteractionProfile.GetPreviewRules(
                     InputActionType.None,
                     InteractionPhase.None,
                     ItemSelectionPhase.Selected))
        {
            ApplyPreviewRule(pr, ctx);
        }
    }

    private InteractionHandleContext CreateHandleContext(
        InputActionType input,
        EInteractionIntentType intentType)
    {
        return new InteractionHandleContext(
            _itemInstance,
            _playerTransform.position,
            _lastPointerPosition,
            _playerState.MoveDirection,
            input,
            intentType);
    }

    private void DisablePreview()
    {
        _previewBundle?.Preview?.Hide();
        _previewBundle = null;
    }

    private void ApplyFeedback(
        InteractionHandleContext ctx,
        InteractionFeedback feedback,
        Vector2? lookDirection = null)
    {
        // ---------- outcome gate ----------
        Debug.Log("ApplyFeedback" + !feedback.HasCost);

        if (!feedback.HasCost)
            return;

        Debug.Log("ApplyFeedback");

        // ---------- rotate ----------
        if (lookDirection.HasValue)
        {
            _playerState.Look(lookDirection.Value.normalized);
        }

        // ---------- energy ----------
        if (feedback.EnergyCost > 0)
        {
            _interactor.TryExecute(
                new ConsumeEnergyCommand(feedback.EnergyCost));
        }

        // ---------- item ----------
        if (feedback.ItemCost > 0 && ctx.ItemInstance != null)
        {
            _interactor.TryExecute(
                new ConsumeItemCommand(
                    ctx.ItemInstance.Data,
                    feedback.ItemCost));
        }

        string key = feedback.IntentType.ToString();

        if (feedback.PlayerCooldown > 0f)
        {
            // _playerCooldown.ApplyCooldown(
            //     key,
            //     feedback.PlayerCooldown);
        }

        if (ctx.ItemInstance is PlantItemInstance plant)
        {
            plant.CooldownOwner.ApplyCooldown(
                key,
                plant.GetStat(EItemStatType.Cooldown));
        }
    }
}