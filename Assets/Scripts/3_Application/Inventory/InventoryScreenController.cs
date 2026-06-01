#nullable enable

using UnityEngine;

// Inventory UI orchestrator — เปิด/ปิดผ่าน state machine (gameApplication.ToggleInventory triggers ChangeState)
// IGameSystem.Enter/Exit ถูกเรียกตอน state เข้า/ออก Inventory state
//
// IModalUI: push stack ตอน Open (PausesGame=false — Inventory ไม่ pause โลก, แต่ block input ผ่าน HasAny)
// HandleDismiss → ChangeState(Gameplay) → state machine fires Exit → pop stack (recursive-safe เพราะ Pop เป็น expression เดียว)
public sealed class InventoryScreenController : IGameSystem, IModalUI
{
  private readonly IInventoryUIRoot _uiRoot;
  private readonly InventoryController _inventoryController;
  private readonly ModalUIStack _modalStack;
  private readonly GameStateMachine _stateMachine;
  private readonly IAudioService? _audio;
  private readonly IInventorySoundConfig? _soundConfig;
  private bool _isOpen;

  public InventoryScreenController(
    IInventoryUIRoot uiRoot,
    InventoryController inventoryController,
    ModalUIStack modalStack,
    GameStateMachine stateMachine,
    IAudioService? audio = null,
    IInventorySoundConfig? soundConfig = null)
  {
    _uiRoot = uiRoot;
    _inventoryController = inventoryController;
    _modalStack = modalStack;
    _stateMachine = stateMachine;
    _audio = audio;
    _soundConfig = soundConfig;
  }

  // ==========================
  // IModalUI
  // ==========================

  public bool IsOpen => _isOpen;
  public bool PausesGame => false;  // Inventory ไม่ pause — โลกเดินต่อระหว่างเปิด
  public void HandleDismiss()
  {
    if (!_isOpen) return;
    _stateMachine.ChangeState(EGameState.Gameplay);  // → fires Exit → Close → Pop stack
  }

  // ==========================
  // Lifecycle (IGameSystem)
  // ==========================

  public void Open()
  {
    if (_isOpen) return;
    _isOpen = true;
    _uiRoot.Open();
    _inventoryController.OnInventoryOpened();
    _modalStack.Push(this);
    PlaySfx(_soundConfig?.OnOpen);
  }

  public void Close()
  {
    if (!_isOpen) return;
    _isOpen = false;
    _uiRoot.Close();
    _inventoryController.OnInventoryClosed();
    _modalStack.Pop(this);
    PlaySfx(_soundConfig?.OnClose);
  }

  public void Enter() => Open();
  public void Exit() => Close();

  public void Update(float dt) { }
  public void FixedUpdate(float dt) { }

  private void PlaySfx(SoundKey? key)
  {
    if (_audio == null || key == null) return;
    _audio.PlaySFX(key, Vector3.zero);
  }
}
