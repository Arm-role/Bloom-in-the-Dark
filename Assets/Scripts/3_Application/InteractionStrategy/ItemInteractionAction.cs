using System;
using UnityEngine;

// Orchestrator for item interaction: routes input phases, tracks the selected
// item, and delegates preview to InteractionPreviewController and action
// execution to InteractionActionRunner.
public sealed class ItemInteractionAction : IDispose, IGameStateListener
{
  private IItemInstance _itemInstance;
  private IItemInteractionCapability _itemInteractionCapability;
  private ItemStrategyBundle _globalBundle;

  private readonly InteractionHandleService _interactionHandleService;
  private readonly Transform _owner;
  private readonly PlayerInteractor _interactor;
  private readonly PlayerState _playerState;
  private readonly IDragDropController _dragDropController;
  private readonly IGlobalInteractionConfig _globalConfig;

  private readonly InteractionPreviewController _preview;
  private readonly InteractionActionRunner _actionRunner;

  private Vector2 _lastPointerPosition;

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
    _interactor = interactor;
    _playerState = playerState;
    _owner = playerTransform;
    _dragDropController = dragDropController;
    _globalConfig = globalConfig;

    _preview = new InteractionPreviewController(
      _interactionHandleService, CreateHandleContext);

    _actionRunner = new InteractionActionRunner(
      interactor, cooldownContainer, playerTransform, costResolver,
      playerState, animationTagService, animationSystem, executor);

    _dragDropController.OnInteraction += ProcessInteractionContext;

    _globalBundle = _interactionHandleService.Resolve(EItemStrategyType.DirectInteract);
  }

  public void Dispose()
  {
    _dragDropController.OnInteraction -= ProcessInteractionContext;
    _actionRunner.Dispose();
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

    _actionRunner.Execute(intent, bundle, target);
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

    _actionRunner.Execute(intent, bundle, target);
  }

  private IItemInstance GetItemOnSlot()
  {
    var slot = _interactor.GetSelectedSlot();
    if (slot.IsEmpty)
      return _interactor.GetEmptyItem();

    return slot.GetItemInstance();
  }

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
