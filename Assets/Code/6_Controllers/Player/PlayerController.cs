using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
    private Rigidbody2D _rb;

    private IMovement _playerMovement;
    private PlayerState _state;
    private PlayerData _playerData;
    private PlayerEnergy _playerEnergy;
    private ICharacterAnimationView _playerAnimationView;

    public IHealthBarView HealthBarView { get; private set; }
    public IHealthBarView EnergyBarView { get; private set; }

    private IFlashHitView _flashHitView;

    public event Action<GameObject> OnRequestDestruction;

    public bool IsAlive { get; set; } = true;

    private void Awake()
    {   
        _playerAnimationView = GetComponent<ICharacterAnimationView>();
        _flashHitView = GetComponent<IFlashHitView>();

        foreach (var bar in GetComponents<IHealthBarView>())
        {
            if (bar.Name == "HP")
            {
                HealthBarView = bar;
            }
            else if (bar.Name == "Energy")
            {
                EnergyBarView = bar;
            }
        }

        if (_playerAnimationView == null)
        {
            Debug.LogError("Missing a required dependency (IPlayerAnimationView)!", this);
            this.enabled = false;
            return;
        }

        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialze(float moveSpeed, PlayerData playerData, PlayerState playerState, PlayerEnergy playerEnergy)
    {
        _playerMovement = new PlayerMovement(moveSpeed);

        _state = playerState;
        _playerData = playerData;
        _playerEnergy = playerEnergy;

        _state.MaxHP = playerData.MaxHealth;
        _state.CurrentHP = playerData.CurrentHealth;

        HealthBarView.Setup(playerData.MaxHealth);
        EnergyBarView.Setup(playerEnergy.MaxEnergy);

        _state.OnMoveDirection += _playerAnimationView.SetMoveDirection;
        _state.OnLookDirection += _playerAnimationView.SetLookirection;

        _state.OnDied += OnDied;

        _playerEnergy.OnAmmountRemoveChanged += EnergyChange;
    }

    private void OnDisable()
    {
        _state.OnMoveDirection -= _playerAnimationView.SetMoveDirection;
        _state.OnLookDirection -= _playerAnimationView.SetLookirection;
    }

    public void ManualUpdate(IPlayerInput playerInput)
    {
        _state.UpdateMoveDirection(playerInput.MoveDirection);
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
        _state.TakeDamage(damage);

        if (_state.IsDead)
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

        if (HealthBarView != null)
        {
            HealthBarView?.TakeDamage(damage);
        }
    }

    private void EnergyChange(float ammount)
    {
        if (EnergyBarView != null)
        {
            EnergyBarView?.TakeDamage(ammount);
        }
    }

    private void OnDied()
    {
        RequestDestruction();
        _state.OnDied -= OnDied;
    }
}
