using UnityEngine;

public class ItemInteractionAction
{
  //----ObjectInteraction----//
  private IItemInstance _itemInstance;

  private readonly InteractionHandleService _interactionHandleService;
  private readonly InteractionCostResolver _costResolver;
  private readonly WorldInteractionExecutor _executor;

  private readonly Transform _playerTransform;
  private readonly PlayerInteractor _interactor;
  private readonly PlayerState _playerState;

  private readonly IDragDropController _dragDropController;
  private readonly ParticalService _particalService;

  private ItemInteractionCapability _itemInteractionCapability;
  private ItemStrategyBundle _globalBundle;
  private ItemStrategyBundle _previewBundle;

  private Vector2 _lastPointerPosition;

  public ItemInteractionAction(
    InteractionHandleService interactionHandleService,
    InteractionCostResolver costResolver,
    WorldInteractionExecutor executor,
    Transform playerTransform,
    PlayerInteractor interactor,
    PlayerState playerState,
    IDragDropController dragDropController,
    ParticalService particalService)
  {
    _interactionHandleService = interactionHandleService;
    _costResolver = costResolver;
    _executor = executor;

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

    if (_itemInstance == null || _itemInteractionCapability == null)
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

    var ctx = CreateHandleContext(input);

    foreach (var pr in _itemInteractionCapability.GetPreviewRules(
               input, phase, ItemSelectionPhase.Selected))
    {
      ApplyPreviewRule(pr, ctx);
    }
  }

  private void TickPreview()
  {
    var ctx = CreateHandleContext(
      InputActionType.None);

    foreach (var pr in _itemInteractionCapability.GetPreviewRules(
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

    var bundle = _interactionHandleService.Resolve(rule.Strategy);
    if (bundle == null)
      return;

    if (_previewBundle == null || _previewBundle != bundle)
    {
      DisablePreview();
      _previewBundle = bundle;
      _previewBundle?.Preview?.Setup();
    }

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

    if (!_itemInteractionCapability.TryGetInteractionRule(input, phase, ctx, out var rule))
      return;

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
    Debug.Log("ExecuteTargetedGlobal");

    var bundle = _globalBundle;

    var config = bundle.Targeting.ConfigProvider.Create(ctx);
    var target = bundle.Targeting.Strategy.Resolve(ctx, config);

    if (!target.IsValid)
      return;

    var intent = ctx.ToIntent(EInteractionIntentType.Harvest);

    ExecuteAction(intent, bundle, target);
  }

  private async void ExecuteAction(
    InteractionIntent intent,
    ItemStrategyBundle bundle,
    TargetResult targetResult)
  {
    if (_interactor.IsBusy())
      return;

    InteractionResult result = await bundle.Action.Execute(intent, targetResult);

    if (result.Outcome != InteractionOutcome.Consumed)
      return;

    var itemData = intent.SourceItem?.Data;
    var categoryData = new ItemCategoryData(
      itemData.Category, itemData.Role);

    if (_costResolver.TryResolve(
          intent.Type,
          categoryData,
          result.TargetType,
          out var feedback) &&
        CanAfford(intent, feedback))
    {
      await _executor.Execute(result.Action, (WorldCell)result.Cell);
      ApplyFeedback(intent, feedback);
    }
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

    DisablePreview();

    _itemInstance = itemInstance;
    _itemInteractionCapability =
      itemInstance?.Data.InteractionCapability as ItemInteractionCapability;

    if (_itemInteractionCapability == null)
      return;

    var ctx = CreateHandleContext(InputActionType.None);

    foreach (var pr in _itemInteractionCapability.GetPreviewRules(
               InputActionType.None,
               InteractionPhase.None,
               ItemSelectionPhase.Selected))
    {
      ApplyPreviewRule(pr, ctx);
    }
  }

  private InteractionHandleContext CreateHandleContext(
    InputActionType input)
  {
    return new InteractionHandleContext(
      _itemInstance,
      _playerTransform.position,
      _lastPointerPosition,
      _playerState.MoveDirection,
      input);
  }

  private void DisablePreview()
  {
    _previewBundle?.Preview?.Hide();
    _previewBundle = null;
  }

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
    Vector2? lookDirection = null)
  {

    // ---------- outcome gate ---------
    if (!feedback.HasCost)
      return;

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
    if (feedback.ItemCost > 0 && intent.SourceItem != null)
    {
      _interactor.TryExecute(
        new ConsumeItemCommand(
          intent.SourceItem.Data,
          feedback.ItemCost));
    }

    string key = feedback.IntentType.ToString();

    if (feedback.PlayerCooldown > 0f)
    {
      _interactor.TryStartAction(
        key,
        feedback.PlayerCooldown);
    }
    //
    // if (intent.SourceItem is PlantItemInstance plant)
    // {
    //   plant.CooldownOwner.ApplyCooldown(
    //     key,
    //     plant.GetStat(EItemStatType.Cooldown));
    // }
  }
}