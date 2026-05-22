using System;
using UnityEngine;

public sealed class ItemInteractionAction : IDispose, IGameStateListener
{
  //----ObjectInteraction----//
  private IItemInstance _itemInstance;

  private readonly InteractionHandleService _interactionHandleService;
  private readonly WorldInteractionExecutor _executor;

  private readonly Transform _owner;
  private readonly PlayerInteractor _interactor;
  private readonly PlayerState _playerState;

  private readonly IDragDropController _dragDropController;

  private readonly InteractionCostResolver _costResolver;
  private readonly CharacterAnimationSystem _playerAnimationSystem;
  private readonly CharacterAnimationTagService _animationTagService;
  private readonly CooldownContainer _cooldownContainer;

  private IItemInteractionCapability _itemInteractionCapability;
  private ItemStrategyBundle _globalBundle;

  private readonly InteractionPreviewController _preview;

  private readonly IGlobalInteractionConfig _globalConfig;

  private Vector2 _lastPointerPosition;
  private InteractionExecutionPlan _pendingPlan;
  private InteractionFeedback _currentFeedback;

  public ItemInteractionAction(
    InteractionHandleService interactionHandleService,
    WorldInteractionExecutor executor,
    Transform playerTransform,
    PlayerInteractor interactor,
    PlayerState playerState,
    IDragDropController dragDropController,
    InteractionCostResolver costResolver,
    CharacterAnimationSystem animationSystem,
    CharacterAnimationTagService animationTagService,
    CooldownContainer cooldownContainer,
    IGlobalInteractionConfig globalConfig)
  {
    _interactionHandleService = interactionHandleService;
    _executor = executor;

    _interactor = interactor;
    _playerState = playerState;
    _owner = playerTransform;
    _dragDropController = dragDropController;

    _costResolver = costResolver;
    _playerAnimationSystem = animationSystem;
    _animationTagService = animationTagService;
    _cooldownContainer = cooldownContainer;
    _globalConfig = globalConfig;

    _preview = new InteractionPreviewController(
      _interactionHandleService, CreateHandleContext);

    _dragDropController.OnInteraction += ProcessInteractionContext;
    _playerAnimationSystem.RaiseImpact += CommitPendingAction;
    _playerAnimationSystem.RaiseFinished += CommitPendingAction;

    _globalBundle = _interactionHandleService.Resolve(EItemStrategyType.DirectInteract);
  }

  public void Dispose()
  {
    _dragDropController.OnInteraction -= ProcessInteractionContext;
    _playerAnimationSystem.RaiseImpact -= CommitPendingAction;
    _playerAnimationSystem.RaiseFinished -= CommitPendingAction;
  }

  // ออกจาก Gameplay (popup upgrade/inventory/pause เปิด) → ซ่อน preview indicator
  // gameplay loop หยุด tick ตอนนั้น preview จะไม่ถูก update อีก ถ้าไม่ซ่อนตรงนี้ indicator ค้าง
  public void OnGameStateChanged(EGameState state)
  {
    if (state != EGameState.Gameplay)
      _preview.Disable();
  }

  private void ProcessInteractionContext(InteractionContext result)
  {
    SyncState(result);

    if (_itemInstance == null || _itemInteractionCapability == null)
    {
      ProcessPhases(result, HandleGlobalInteraction);
      return;
    }

    ProcessPhases(result, _preview.Handle);
    _preview.Tick();
    ProcessPhases(result, HandleInteraction);
  }

  private void SyncState(InteractionContext result)
  {
    if (result.UseSourceItem)
      OnItemChanged(GetItemOnSlot());

    if (result.LastPointerPosition.HasValue)
      _lastPointerPosition = result.LastPointerPosition.Value;
  }

  private static void ProcessPhases(
    InteractionContext result,
    Action<InputActionType, InteractionPhase> handler)
  {
    handler(result.Pressed, InteractionPhase.Pressed);
    handler(result.Held, InteractionPhase.Held);
    handler(result.Released, InteractionPhase.Released);
  }

  private void HandleInteraction(
    InputActionType input,
    InteractionPhase phase)
  {
    if (input == InputActionType.None)
      return;

    if (_dragDropController.CurrentHoverState.HasFlag(HoverState.UI))
      return;

    var ctx = new InteractionConditionContext
    {
      Pressed = phase == InteractionPhase.Pressed ? input : InputActionType.None,
      Held = _dragDropController.CurrentHeldActions,
      Released = phase == InteractionPhase.Released ? input : InputActionType.None,
      IsSkillPreviewActive = _preview.IsActive,
      Item = _itemInstance
    };

    if (!_itemInteractionCapability.TryGetInteractionRule(input, phase, ctx, out var rule))
    {
      HandleGlobalInteraction(input, phase);
      return;
    }

    ExecuteTargeted(rule, input);
  }

  private void HandleGlobalInteraction(
    InputActionType input,
    InteractionPhase phase)
  {
    if (input == InputActionType.None)
      return;

    var handleContext = CreateHandleContext(input);
    ExecuteTargetedGlobal(handleContext);
  }

  private void ExecuteTargeted(
    InteractionRule rule,
    InputActionType input)
  {
    var ctx = CreateHandleContext(input);

    // ---------- EXECUTION ----------

    // Strategy = None → ไม่ใช่ item action
    if (rule.Strategy == EItemStrategyType.None)
    {
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
    var intent = ctx.ToIntent(rule.IntentType);

    var validator = bundle.Targeting.Validator?.Validate(ctx, target);

    bool isValid =
      target.IsValid &&
      (validator?.IsValid ?? true) &&
      bundle.Action.CanExecute(intent, target);

    if (!isValid)
    {
      if (rule.Fallback == InteractionFallback.Global)
        ExecuteTargetedGlobal(ctx);

      return;
    }

    ExecuteAction(intent, bundle, target);
  }

  private void ExecuteTargetedGlobal(InteractionHandleContext ctx)
  {

    var bundle = GetGlobalBundle();

    var config = bundle.Targeting.ConfigProvider.Create(ctx);
    var target = bundle.Targeting.Strategy.Resolve(ctx, config);

    if (!target.IsValid)
      return;

    var intentType = _globalConfig.Resolve(_itemInstance);
    var intent = ctx.ToIntent(intentType);

    ExecuteAction(intent, bundle, target);
  }



  // =======================
  // ExecuteAction
  // =======================


  private async void ExecuteAction(
    InteractionIntent intent,
    ItemStrategyBundle bundle,
    TargetResult targetResult)
  {
    try
    {
      var item = intent.SourceItem;
      var itemData = item.Data;

      bool isBusy = _interactor.IsBusy();
      bool isCooldown = _cooldownContainer.IsOnCooldown(itemData.Name);
      bool hasPendingPlan = _pendingPlan != null;

      if (isBusy || isCooldown || hasPendingPlan)
        return;

      // ---- ActionPlan ----

      var plan = await bundle.Action.Prepare(_owner.gameObject, intent, targetResult);

      if (!_costResolver.TryResolve(
           plan.Intent.Type,
           intent.SourceItem.Data,
           plan.TargetMask,
           out var feedback))
        return;

      if (!CanAfford(plan.Intent, feedback))
        return;

      _pendingPlan = plan;
      _currentFeedback = feedback;

      if (intent.Direction.HasValue)
        _playerState.Look(intent.Direction.Value.normalized);

      // ---- Animation ----

      var request = new AnimationRequest
      {
        Intent = intent.Type,
        ItemDefinition = intent.SourceItem.Data,
        TargetMask = plan.TargetMask,
        Direction = intent.Direction.HasValue ? intent.Direction.Value : Vector2.zero
      };

      if (_currentFeedback.PlayerCooldown > 0f)
      {
        _interactor.TryStartAction(
             _pendingPlan.Intent.Type.ToString(),
            _currentFeedback.PlayerCooldown);
      }

      if (_animationTagService.TryResolve(request, out var tag))
      {
        var command = new CharacterAnimationCommand(
          tag,
          request.Direction);

        if (!_playerAnimationSystem.Handle(command))
          CommitPendingAction();

        return;
      }

      CommitPendingAction();
    }
    catch (System.Exception e)
    {
      Debug.LogError($"[Interaction] ExecuteAction failed: {e}");
      _pendingPlan = null;
    }
  }

  private async void CommitPendingAction()
  {
    var plan = _pendingPlan;

    if (plan == null)
      return;

    _pendingPlan = null;

    try
    {
      var result = await plan.Commit();

      if (result.Outcome != InteractionOutcome.Consumed)
        return;

      await _executor.Execute(result.Action, (WorldCell)result.Cell);

      ApplyFeedback(plan.Intent, _currentFeedback, result);
    }
    catch (System.Exception e)
    {
      Debug.LogError($"[Interaction] CommitPendingAction failed: {e}");
    }
  }

  private IItemInstance GetItemOnSlot()
  {
    var slot = _interactor.GetSelectedSlot();
    if (slot.IsEmpty)
      return _interactor.GetEmptyItem();

    return slot.GetItemInstance();
  }

  // =======================
  // Feedback
  // =======================

  private bool CanAfford(
    InteractionIntent intent,
    InteractionFeedback feedback)
  {
    // ---------- outcome gate ----------

    if (feedback.EnergyCost > 0 &&
        !_interactor.CanExecute(
          new ConsumeEnergyCommand(feedback.EnergyCost)))
      return false;

    // ---------- item ----------
    if (feedback.ItemCost > 0 && intent.SourceItem == null)
      return false;

    return true;
  }



  private void ApplyFeedback(
    InteractionIntent intent,
    InteractionFeedback feedback,
    InteractionResult interactionResult)
  {
    // ---------- energy (config only) ----------
    if (feedback.EnergyCost > 0)
    {
      _interactor.TryExecute(
        new ConsumeEnergyCommand(feedback.EnergyCost));
    }

    // ---------- item ----------
    // Action-declared cost takes priority; config cost is the fallback for
    // actions that don't carry their own (e.g. skill attacks).
    int itemCost = interactionResult.Cost.HasItemCost
      ? interactionResult.Cost.ItemCost
      : feedback.ItemCost;

    bool actionGaveRewards = interactionResult.Action?.ItemRewards?.Count > 0;

    if (itemCost > 0 && intent.SourceItem != null && !actionGaveRewards)
    {
      _interactor.TryExecute(
        new ConsumeItemCommand(
          intent.SourceItem.Data,
          itemCost));
    }

    // ---------- cooldown ----------
    if (interactionResult.ItemCooldown.HasCost)
    {
      _cooldownContainer.TryApply(
        interactionResult.ItemCooldown.Key,
        interactionResult.ItemCooldown.Duration);
    }
  }

  // =======================
  // Helper
  // =======================

  private void OnItemChanged(IItemInstance itemInstance)
  {
    if (itemInstance == _itemInstance)
      return;

    _itemInstance = itemInstance;
    _itemInteractionCapability = itemInstance?.Data.InteractionCapability;
    _preview.SetProvider(_itemInteractionCapability);
  }

  private ItemStrategyBundle GetGlobalBundle()
  {
    if (_globalBundle == null)
      _globalBundle = _interactionHandleService.Resolve(EItemStrategyType.DirectInteract);

    return _globalBundle;
  }

  private InteractionHandleContext CreateHandleContext(
    InputActionType input)
  {
    return new InteractionHandleContext(
      _itemInstance,
      _owner.position,
      _lastPointerPosition,
      _playerState.MoveDirection,
      input);
  }


}