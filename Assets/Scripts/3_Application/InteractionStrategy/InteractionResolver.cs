#nullable enable
using System;

// Resolves an input phase into a concrete interaction: picks the strategy
// bundle, resolves + validates the target, then hands the ready action to
// InteractionActionRunner. Falls back to the global interaction when needed.
public sealed class InteractionResolver
{
  private readonly InteractionHandleService _handleService;
  private readonly IDragDropController _dragDropController;
  private readonly IGlobalInteractionConfig _globalConfig;
  private readonly InteractionActionRunner _actionRunner;
  private readonly Func<bool> _isPreviewActive;
  private readonly Func<InputActionType, InteractionHandleContext> _createContext;

  private IItemInstance? _itemInstance;
  private IItemInteractionCapability? _capability;
  private ItemStrategyBundle _globalBundle;

  public InteractionResolver(
    InteractionHandleService handleService,
    IDragDropController dragDropController,
    IGlobalInteractionConfig globalConfig,
    InteractionActionRunner actionRunner,
    Func<bool> isPreviewActive,
    Func<InputActionType, InteractionHandleContext> createContext)
  {
    _handleService = handleService;
    _dragDropController = dragDropController;
    _globalConfig = globalConfig;
    _actionRunner = actionRunner;
    _isPreviewActive = isPreviewActive;
    _createContext = createContext;

    _globalBundle = _handleService.Resolve(EItemStrategyType.DirectInteract);
  }

  // เรียกจาก orchestrator เมื่อ item ที่เลือกเปลี่ยน
  public void SetItem(IItemInstance? itemInstance, IItemInteractionCapability? capability)
  {
    _itemInstance = itemInstance;
    _capability = capability;
  }

  public void HandleInteraction(InputActionType input, InteractionPhase phase)
  {
    if (input == InputActionType.None)
      return;

    if (_dragDropController.CurrentHoverState.HasFlag(HoverState.UI))
      return;

    if (_capability == null)
      return;

    var ctx = new InteractionConditionContext
    {
      Pressed = phase == InteractionPhase.Pressed ? input : InputActionType.None,
      Held = _dragDropController.CurrentHeldActions,
      Released = phase == InteractionPhase.Released ? input : InputActionType.None,
      IsSkillPreviewActive = _isPreviewActive(),
      Item = _itemInstance
    };

    if (!_capability.TryGetInteractionRule(input, phase, ctx, out var rule))
    {
      HandleGlobalInteraction(input, phase);
      return;
    }

    ExecuteTargeted(rule, input);
  }

  public void HandleGlobalInteraction(InputActionType input, InteractionPhase phase)
  {
    if (input == InputActionType.None)
      return;

    var handleContext = _createContext(input);
    ExecuteTargetedGlobal(handleContext);
  }

  private void ExecuteTargeted(InteractionRule rule, InputActionType input)
  {
    var ctx = _createContext(input);

    // Strategy = None → ไม่ใช่ item action
    if (rule.Strategy == EItemStrategyType.None)
    {
      if (rule.Fallback == InteractionFallback.Global)
        ExecuteTargetedGlobal(ctx);

      return;
    }

    var bundle = _handleService.Resolve(rule.Strategy);
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

  private ItemStrategyBundle GetGlobalBundle()
  {
    if (_globalBundle == null)
      _globalBundle = _handleService.Resolve(EItemStrategyType.DirectInteract);

    return _globalBundle;
  }
}
