using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private ICharacterAnimationView _playerAnimationView;
    private IMovement _playerMovement;
    private PlayerData _playerData;

    private void Awake()
    {
        _playerAnimationView = GetComponent<ICharacterAnimationView>();

        if (_playerAnimationView == null)
        {
            Debug.LogError("Missing a required dependency (IPlayerAnimationView)!", this);
            this.enabled = false;
            return;
        }

        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialze(float moveSpeed, PlayerData playerData)
    {
        _playerMovement = new PlayerMovement(moveSpeed);
        _playerData = playerData;

        _playerData.OnMoveDirection += _playerAnimationView.SetMoveDirection;
        _playerData.OnLookDirection += _playerAnimationView.SetLookirection;
    }

    private void OnDisable()
    {
        _playerData.OnMoveDirection -= _playerAnimationView.SetMoveDirection;
        _playerData.OnLookDirection -= _playerAnimationView.SetLookirection;
    }

    public void ManualUpdate(IPlayerInput playerInput)
    {
        _playerData.UpdateMoveDirection(playerInput.MoveDirection);
    }

    public void ManualFixedUpdate(IPlayerInput playerInput)
    {
        Vector2 direction = playerInput.MoveDirection;

        Vector2 velocity = _playerMovement.CalculateVelocity(direction);
        _rb.velocity = velocity;
    }
}
