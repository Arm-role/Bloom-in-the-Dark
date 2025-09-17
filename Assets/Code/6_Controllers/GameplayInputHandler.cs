using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayInputHandler : MonoBehaviour
{
    private IPlayerInput _playerInput;
    private InputManager _inputManager;

    private PlayerController _playerController;
    private ItemActionController _itemActionController;

    public void Initialize(
        IPlayerInput playerInput,
        InputManager inputManager,
        PlayerController playerController,
        ItemActionController itemActionController)
    {
        _playerInput = playerInput;
        _inputManager = inputManager;
        _playerController = playerController;
        _itemActionController = itemActionController;
    }
    private void Update()
    {
        if (_inputManager.CurrentMode != InputMode.Gameplay) return;
        if (_playerInput == null) return;

        _playerController.ManualUpdate(_playerInput);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        _itemActionController.ManualUpdate(_playerInput);
    }
    private void FixedUpdate()
    {
        if (_inputManager.CurrentMode != InputMode.Gameplay) return;
        if (_playerInput == null) return;

        _playerController?.ManualFixedUpdate(_playerInput);
    }
}
