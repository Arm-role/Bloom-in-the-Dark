using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private IPlayerAnimationView _playerAnimationView; 
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerAnimationView = GetComponent<IPlayerAnimationView>();

        // Validate dependencies
        if (_playerAnimationView == null)
        {
            Debug.LogError("Missing a required dependency (IPlayerAnimationView)!", this);
            this.enabled = false;
            return;
        }

        _rb = GetComponent<Rigidbody2D>();

        // Instantiate logic
    }

    public void Initialze(float moveSpeed)
    {
        _playerMovement = new PlayerMovement(moveSpeed);
    }
    
    public void ManualUpdate(IPlayerInput playerInput)
    {
        _playerAnimationView.SetMoveDirection(playerInput.MoveDirection);
    }

    public void ManualFixedUpdate(IPlayerInput playerInput)
    {
        Vector2 direction = playerInput.MoveDirection;
        Vector2 velocity = _playerMovement.CalculateVelocity(direction);
        _rb.velocity = velocity;
    }
}
