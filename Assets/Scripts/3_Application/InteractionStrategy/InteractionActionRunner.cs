#nullable enable
using System;
using System.Threading.Tasks;
using UnityEngine;

// Runs a resolved interaction: prepares the action plan, checks affordability,
// drives the player animation, then commits and applies feedback.
// Owns the pending-plan state and the animation-commit event subscription.
public sealed class InteractionActionRunner : IDisposable
{
  private readonly PlayerInteractor _interactor;
  private readonly CooldownContainer _cooldownContainer;
  private readonly Transform _owner;
  private readonly InteractionCostResolver _costResolver;
  private readonly PlayerState _playerState;
  private readonly CharacterAnimationTagService _animationTagService;
  private readonly CharacterAnimationSystem _animationSystem;
  private readonly WorldInteractionExecutor _worldExecutor;

  private InteractionExecutionPlan? _pendingPlan;
  private InteractionFeedback _currentFeedback;

  public InteractionActionRunner(
    PlayerInteractor interactor,
    CooldownContainer cooldownContainer,
    Transform owner,
    InteractionCostResolver costResolver,
    PlayerState playerState,
    CharacterAnimationTagService animationTagService,
    CharacterAnimationSystem animationSystem,
    WorldInteractionExecutor worldExecutor)
  {
    _interactor = interactor;
    _cooldownContainer = cooldownContainer;
    _owner = owner;
    _costResolver = costResolver;
    _playerState = playerState;
    _animationTagService = animationTagService;
    _animationSystem = animationSystem;
    _worldExecutor = worldExecutor;

    _animationSystem.RaiseImpact += OnAnimationCommit;
    _animationSystem.RaiseFinished += OnAnimationCommit;
  }

  public void Dispose()
  {
    _animationSystem.RaiseImpact -= OnAnimationCommit;
    _animationSystem.RaiseFinished -= OnAnimationCommit;
  }

  // Fire-and-forget entry point. The async workflow is fully encapsulated as
  // `async Task` and every path is guarded by try/catch, so the discarded Task
  // never faults — unlike the old fire-and-forget void method whose escaping
  // exceptions could crash the app.
  public void Execute(
    InteractionIntent intent,
    ItemStrategyBundle bundle,
    TargetResult targetResult)
  {
    _ = ExecuteAsync(intent, bundle, targetResult);
  }

  private async Task ExecuteAsync(
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
          plan.Intent.Type.ToString(),
          _currentFeedback.PlayerCooldown);
      }

      if (_animationTagService.TryResolve(request, out var tag))
      {
        var command = new CharacterAnimationCommand(tag, request.Direction);

        if (!_animationSystem.Handle(command))
          _ = CommitPendingAsync();

        return;
      }

      _ = CommitPendingAsync();
    }
    catch (Exception e)
    {
      Debug.LogError($"[Interaction] ExecuteAction failed: {e}");
      _pendingPlan = null;
    }
  }

  private void OnAnimationCommit()
  {
    _ = CommitPendingAsync();
  }

  private async Task CommitPendingAsync()
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

      await _worldExecutor.Execute(result.Action, (WorldCell)result.Cell);

      ApplyFeedback(plan.Intent, _currentFeedback, result);
    }
    catch (Exception e)
    {
      Debug.LogError($"[Interaction] CommitPendingAction failed: {e}");
    }
  }

  private bool CanAfford(
    InteractionIntent intent,
    InteractionFeedback feedback)
  {
    if (feedback.EnergyCost > 0 &&
        !_interactor.CanExecute(new ConsumeEnergyCommand(feedback.EnergyCost)))
      return false;

    if (InteractionCostPolicy.LacksRequiredItem(feedback, intent.SourceItem != null))
      return false;

    return true;
  }

  private void ApplyFeedback(
    InteractionIntent intent,
    InteractionFeedback feedback,
    InteractionResult interactionResult)
  {
    if (feedback.EnergyCost > 0)
      _interactor.TryExecute(new ConsumeEnergyCommand(feedback.EnergyCost));

    int itemCost = InteractionCostPolicy.ResolveItemCost(interactionResult, feedback);
    bool actionGaveRewards = interactionResult.Action?.ItemRewards?.Count > 0;

    if (InteractionCostPolicy.ShouldConsumeItem(
          itemCost, intent.SourceItem != null, actionGaveRewards))
    {
      _interactor.TryExecute(
        new ConsumeItemCommand(intent.SourceItem.Data, itemCost));
    }

    if (interactionResult.ItemCooldown.HasCost)
    {
      _cooldownContainer.TryApply(
        interactionResult.ItemCooldown.Key,
        interactionResult.ItemCooldown.Duration);
    }
  }
}
