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

  [SerializeField] private Transform AnimationViewRoot;

  private Rigidbody2D _rb;

  private IPlayerInput _playerInput;
  private IStatDatabase _statDatabase;

  private IMovement _playerMovement;
  private PlayerState _playerState;
  private IStatService _statService;

  private HealthResource _playerHealth;

  private KnockbackSimulator _knockback;

  private BarPresenter<HealthResource> _healthPresenter;
  private BarPresenter<PlayerEnergy> _energyPresenter;

  private CharacterAnimationSystem _playerAnimationSystem;

  public PlayerEnergy PlayerEnergy { get; private set; }

  public PlayerInteractor Interactor { get; private set; }

  private IBarView BarView { get; set; }
  private IBarView EnergyBarView { get; set; }

  private IFlashHitView _flashHitView;

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<GameObject> OnRequestDestruction;

  public bool IsAlive { get; set; } = true;

  private void Awake()
  {
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

    _rb = GetComponent<Rigidbody2D>();
  }

  public void Initialize(
    IPlayerInput playerInput,
    IStatDatabase statDatabase,
    PlayerState playerState,
    IStatService statService,
    HealthResource playerHealth,
    PlayerEnergy playerEnergy,
    PlayerInteractor interactor,
    CharacterAnimationSystem animationSystem)
  {
    _playerInput = playerInput;
    _statDatabase = statDatabase;
    _playerState = playerState;
    _statService = statService;

    _playerHealth = playerHealth;

    _knockback = GetComponent<KnockbackSimulator>();

    Interactor = interactor;

    _playerAnimationSystem = animationSystem;
    _playerAnimationSystem.Initializa(AnimationViewRoot.GetComponent<ICharacterAnimationView>());

    _healthPresenter = new BarPresenter<HealthResource>(playerHealth, BarView);
    _energyPresenter = new BarPresenter<PlayerEnergy>(playerEnergy, EnergyBarView);

    _playerMovement = new PlayerMovement(_statDatabase.MoveSpeed, _statService);

    _playerState.OnMoveDirection += _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection += _playerAnimationSystem.SetLookDirection;

    OnDamaged += _playerAnimationSystem.HandleDamage;
  }

  private void OnDisable()
  {
    _playerState.OnMoveDirection -= _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection -= _playerAnimationSystem.SetLookDirection;

    OnDamaged -= _playerAnimationSystem.HandleDamage;
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

    Interactor.TryExecute(
      new TakeDamageCommand(damage)
    );

    if (force != 0 && duration != 0)
      _knockback?.ApplyKnockback(dir, force, duration);

    bool isDead = !_playerHealth.IsAlive;

    var result = new CharacterDamageResult(
      damage,
      dir,
      isDead);

    OnDamaged?.Invoke(result);

    if (isDead)
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
