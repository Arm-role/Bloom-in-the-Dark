using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
    private Rigidbody2D _rb;

    private IMovement _playerMovement;
    private PlayerData _data;
    private ICharacterAnimationView _playerAnimationView;
    private IFlashHitView _flashHitView;

    public event Action<GameObject> OnRequestDestruction;

    public bool IsAlive { get; set; } = true;

    private void Awake()
    {   
        _playerAnimationView = GetComponent<ICharacterAnimationView>();
        _flashHitView = GetComponent<IFlashHitView>();

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
        _data = playerData;

        _data.MaxHP = 99999;
        _data.CurrentHP = 99999;

        _data.OnMoveDirection += _playerAnimationView.SetMoveDirection;
        _data.OnLookDirection += _playerAnimationView.SetLookirection;

        _data.OnDied += OnDied;
    }

    private void OnDisable()
    {
        _data.OnMoveDirection -= _playerAnimationView.SetMoveDirection;
        _data.OnLookDirection -= _playerAnimationView.SetLookirection;
    }

    public void ManualUpdate(IPlayerInput playerInput)
    {
        _data.UpdateMoveDirection(playerInput.MoveDirection);
    }

    public void ManualFixedUpdate(IPlayerInput playerInput)
    {
        Vector2 direction = playerInput.MoveDirection;

        Vector2 velocity = _playerMovement.CalculateVelocity(direction);
        _rb.velocity = velocity;
    }


    public void OnSpawnFromPool(GameObject ob) { }
    public void OnReturnToPool(GameObject ob) { }

    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }

    public void TakeDamage(float damage)
    {
        _data.TakeDamage(damage);

        if (_data.IsDead)
        {
            RequestDestruction();
        }

        if (_flashHitView != null)
        {
            _flashHitView.FlashEffect();
        }

        if(_playerAnimationView != null)
        {
            _playerAnimationView?.PlayHit();
        }
    }

    private void OnDied()
    {
        RequestDestruction();
        _data.OnDied -= OnDied;
    }
}
