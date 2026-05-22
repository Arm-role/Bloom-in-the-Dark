#nullable enable
using System;
using UnityEngine;

// Orchestrator for item interaction: routes input phases and tracks the
// selected item, delegating preview, resolution, and action execution to
// dedicated collaborators.
public sealed class ItemInteractionAction : IDispose, IGameStateListener
{
  private IItemInstance? _itemInstance;
  private IItemInteractionCapability? _itemInteractionCapability;

  private readonly Transform _owner;
  private readonly PlayerInteractor _interactor;
  private readonly PlayerState _playerState;
  private readonly IDragDropController _dragDropController;

  private readonly InteractionPreviewController _preview;
  private readonly InteractionResolver _resolver;
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
    _interactor = interactor;
    _playerState = playerState;
    _owner = playerTransform;
    _dragDropController = dragDropController;

    _actionRunner = new InteractionActionRunner(
      interactor, cooldownContainer, playerTransform, costResolver,
      playerState, animationTagService, animationSystem, executor);

    _preview = new InteractionPreviewController(
      interactionHandleService, CreateHandleContext);

    _resolver = new InteractionResolver(
      interactionHandleService, dragDropController, globalConfig,
      _actionRunner, () => _preview.IsActive, CreateHandleContext);

    _dragDropController.OnInteraction += ProcessInteractionContext;
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
      ProcessPhases(result, _resolver.HandleGlobalInteraction);
      return;
    }

    ProcessPhases(result, _preview.Handle);
    _preview.Tick();
    ProcessPhases(result, _resolver.HandleInteraction);
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
    _resolver.SetItem(_itemInstance, _itemInteractionCapability);
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
