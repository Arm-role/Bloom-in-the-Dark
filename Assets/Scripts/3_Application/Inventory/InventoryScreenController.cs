#nullable enable

using UnityEngine;

public sealed class InventoryScreenController : IGameSystem
{
  private readonly IInventoryUIRoot _uiRoot;
  private readonly InventoryController _inventoryController;
  private readonly IAudioService? _audio;
  private readonly IInventorySoundConfig? _soundConfig;

  public InventoryScreenController(
    IInventoryUIRoot uiRoot,
    InventoryController inventoryController,
    IAudioService? audio = null,
    IInventorySoundConfig? soundConfig = null)
  {
    _uiRoot = uiRoot;
    _inventoryController = inventoryController;
    _audio = audio;
    _soundConfig = soundConfig;
  }

  public void Open()
  {
    _uiRoot.Open();
    _inventoryController.OnInventoryOpened();
    PlaySfx(_soundConfig?.OnOpen);
  }

  public void Close()
  {
    _uiRoot.Close();
    _inventoryController.OnInventoryClosed();
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
