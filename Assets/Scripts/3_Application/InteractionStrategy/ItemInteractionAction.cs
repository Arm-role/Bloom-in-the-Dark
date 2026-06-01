#nullable enable
using System;
using UnityEngine;

// Orchestrator for item interaction: routes input phases and tracks the
// selected item, delegating preview, resolution, and action execution to
// dedicated collaborators.
public sealed class ItemInteractionAction : IDispose
{
  private IItemInstance? _itemInstance;
  private IItemInteractionCapability? _itemInteractionCapability;

  private readonly Transform _owner;
  private readonly PlayerInteractor _interactor;
  private readonly PlayerState _playerState;
  private readonly IDragDropController _dragDropController;
  private readonly PlayerController _playerController;
  private readonly ModalUIStack _modalStack;

  private readonly InteractionPreviewController _preview;
  private readonly InteractionResolver _resolver;
  private readonly InteractionActionRunner _actionRunner;

  private Vector2 _lastPointerPosition;

  // ยิงเมื่อ player ใช้ item ทำ action สำเร็จ (ผ่าน cost + affordability + animation + world executor)
  // ใช้สำหรับ hint/analytics — UI click ไม่ trigger event นี้ (ไม่ผ่าน DragDropController.OnInteraction)
  public event Action? OnActionCommitted;

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
    IGlobalInteractionConfig globalConfig,
    PlayerController playerController,
    ModalUIStack modalStack)
  {
    _interactor = interactor;
    _playerState = playerState;
    _owner = playerTransform;
    _dragDropController = dragDropController;
    _playerController = playerController;
    _modalStack = modalStack;

    _actionRunner = new InteractionActionRunner(
      interactor, cooldownContainer, playerTransform, costResolver,
      playerState, animationTagService, animationSystem, executor);

    _preview = new InteractionPreviewController(
      interactionHandleService, CreateHandleContext);

    _resolver = new InteractionResolver(
      interactionHandleService, dragDropController, globalConfig,
      _actionRunner, () => _preview.IsActive, CreateHandleContext);

    _dragDropController.OnInteraction += ProcessInteractionContext;
    _playerController.OnDamaged += HandlePlayerDamaged;
    _actionRunner.OnCommitted += RaiseActionCommitted;
    // Modal เปิดตัวแรก (0 → 1) → cancel pending action + ซ่อน preview
    // กัน animation ค้างเล่นต่อหลัง modal ปิด (Impact ยิง → action commit ทับโดยไม่ตั้งใจ)
    _modalStack.OnFirstPush += HandleModalOpen;
  }

  public void Dispose()
  {
    _dragDropController.OnInteraction -= ProcessInteractionContext;
    _playerController.OnDamaged -= HandlePlayerDamaged;
    _actionRunner.OnCommitted -= RaiseActionCommitted;
    _modalStack.OnFirstPush -= HandleModalOpen;
    _actionRunner.Dispose();
  }

  private void HandleModalOpen()
  {
    _preview.Disable();
    _actionRunner.CancelPending();
  }

  private void RaiseActionCommitted() => OnActionCommitted?.Invoke();

  // Player โดน hit ก่อน action animation จะยิง RaiseImpact/Finished
  // → action clip ถูก override โดย hit clip → event chain ของ action ไม่ยิง
  // → ถ้าไม่ cancel _pendingPlan จะค้าง block interaction ทั้งหมดถัดไป
  private void HandlePlayerDamaged(CharacterDamageResult _)
    => _actionRunner.CancelPending();

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
