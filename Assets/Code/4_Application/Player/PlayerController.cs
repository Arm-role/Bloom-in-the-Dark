using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController :
  MonoBehaviour,
  IPlayerController,
  IDamageable,
  IDestructible,
  IPoolable<GameObject>
{
  private Rigidbody2D _rb;

  private IPlayerInput _playerInput;

  private IMovement _playerMovement;
  private PlayerState _playerState;

  private HealthResource _playerHealth;

  private KnockbackSimulator _knockback;

  private BarPresenter<HealthResource> _healthPresenter;
  private BarPresenter<PlayerEnergy> _energyPresenter;

  private ICharacterAnimationView _playerAnimationView;

  public PlayerEnergy PlayerEnergy { get; private set; }

  public PlayerInteractor Interactor { get; private set; }

  private IBarView BarView { get; set; }
  private IBarView EnergyBarView { get; set; }

  private IFlashHitView _flashHitView;

  public event Action<GameObject> OnRequestDestruction;

  public bool IsAlive { get; set; } = true;

  private void Awake()
  {
    _playerAnimationView = GetComponent<ICharacterAnimationView>();
    _flashHitView = GetComponent<IFlashHitView>();

    foreach (var bar in GetComponents<IBarView>())
    {
      if (bar.Name == "HP")
      {
        BarView = bar;
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

  public void Initialize(
    IPlayerInput playerInput,
    PlayerData playerData,
    PlayerState playerState,
    HealthResource playerHealth,
    PlayerEnergy playerEnergy,
    PlayerInteractor interactor)
  {
    _playerInput = playerInput;
    _playerState = playerState;

    _knockback = GetComponent<KnockbackSimulator>();

    Interactor = interactor;

    _healthPresenter = new BarPresenter<HealthResource>(playerHealth, BarView);
    _energyPresenter = new BarPresenter<PlayerEnergy>(playerEnergy, EnergyBarView);

    _playerMovement = new PlayerMovement(playerData.MoveSpeed);

    _playerState.OnMoveDirection += _playerAnimationView.SetMoveDirection;
    _playerState.OnLookDirection += _playerAnimationView.SetLookirection;
  }

  private void OnDisable()
  {
    _playerState.OnMoveDirection -= _playerAnimationView.SetMoveDirection;
    _playerState.OnLookDirection -= _playerAnimationView.SetLookirection;
  }

  public void ManualUpdate()
  {
    if (Interactor != null && Interactor.IsBusy())
    {
      _playerState.UpdateMoveDirection(Vector2.zero);
      return;
    }

    _playerState.UpdateMoveDirection(_playerInput.MoveDirection);
  }

  public void ManualFixedUpdate()
  {
    if (Interactor != null && Interactor.IsBusy())
    {
      _rb.velocity = Vector2.zero;
      return;
    }

    if (_knockback != null && _knockback.IsKnockbacking)
      return;

    Vector2 direction = _playerInput.MoveDirection;
    Vector2 velocity = _playerMovement.CalculateVelocity(direction);
    _rb.velocity = velocity;
  }

  public void OnSpawnFromPool(GameObject ob) { }
  public void OnReturnToPool(GameObject ob) { }


  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  public void TakeDamage(float damage, Vector2 dir, float force, float duration)
  {
    _flashHitView?.FlashEffect();
    _playerAnimationView?.PlayHit();

    Interactor.TryExecute(
      new TakeDamageCommand(damage)
    );

    _knockback?.ApplyKnockback(dir, force, duration);

    if (!_playerHealth.IsAlive)
      OnDied();
  }

  public void Heal(float ammount)
  {
    _playerHealth.Heal(ammount);
  }

  public void SetMaxHp(float ammount)
  {
    _playerHealth.SetMax(ammount);
  }

  public void HpFill()
  {
    _playerHealth.Fill();
  }

  private void OnDied()
  {
    RequestDestruction();
  }
}