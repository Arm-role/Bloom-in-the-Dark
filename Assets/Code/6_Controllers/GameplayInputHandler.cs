using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayInputHandler : MonoBehaviour
{
    private IPlayerInput _playerInput;
    private InputManager _inputManager;

    private PlayerController _playerController;
    private DragDropController _dragDropController;

    public void Initialize(
        IPlayerInput playerInput,
        InputManager inputManager,
        PlayerController playerController,
        DragDropController dragDropController)
    {
        _playerInput = playerInput;
        _inputManager = inputManager;
        _playerController = playerController;
        _dragDropController = dragDropController;
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

        Vector3 mouseWorldPos = _playerInput.PointerWorldPosition;
        Vector2 pointerWorldPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        _dragDropController.ManualUpdate(_playerInput, Physics2D.OverlapPointAll(pointerWorldPosition));
    }
    private void FixedUpdate()
    {
        if (_inputManager.CurrentMode != InputMode.Gameplay) return;
        if (_playerInput == null) return;

        _playerController?.ManualFixedUpdate(_playerInput);
    }
}
